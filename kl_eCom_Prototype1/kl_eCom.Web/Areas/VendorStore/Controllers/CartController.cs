using kl_eCom.Web.Areas.VendorStore.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Entities;
using Microsoft.AspNet.Identity;

namespace kl_eCom.Web.Areas.VendorStore.Controllers
{
    public class CartController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: VendorStore/Cart
        public ActionResult Index(bool flag = false, string returnUrl = "",
            bool checkoutErr = false, string removedProd = "", int prodId = 0)
        {
            var cart = GetCart();
            var total = 0.0f;

            var prices = new Dictionary<CartItem, string>();
            var names = new Dictionary<CartItem, string>();

            ViewBag.Flag = flag;
            if (flag)
            {
                ViewBag.ProdName = removedProd;
                ViewBag.StockId = prodId;
            }

            ViewBag.CheckoutErr = checkoutErr;

            foreach (var itm in cart.CartItems)
            {
                if (itm.StockId != null)
                {
                    var stock = db.Stocks.FirstOrDefault(m => m.Id == itm.StockId);
                    var cost = itm.Qty * stock.Price;
                    var prod = db.Products.FirstOrDefault(m => m.Id == stock.ProductId);
                    names.Add(itm, prod.Name);
                    prices.Add(itm, cost.ToString());
                    total += cost;
                }
                else
                {
                    var cartItm = db.CartItems
                                     .Include(m => m.Constraint)
                                     .Include(m => m.Constraint.Discount)
                                     .Include(m => m.Constraint.BundledItems)
                                     .FirstOrDefault(m => m.Id == itm.Id);

                    var bundledItems = cartItm
                                        .Constraint
                                        .BundledItems
                                        .ToList();

                    var totalPrice = 0.0f;

                    names.Add(itm, cartItm.Constraint.Discount.Name + "\n(");
                    foreach (var bndldItm in bundledItems)
                    {
                        var bndl = db.BundledItems
                                           .Include(m => m.Stock)
                                           .Include(m => m.Stock.Product)
                                           .FirstOrDefault(m => m.Id == bndldItm.Id);

                        names[itm] += bndl.Stock.Product.Name;

                        totalPrice += bndl.Stock.Price;

                        if (bundledItems.Last() != bndldItm)
                        {
                            names[itm] += ", ";
                        }
                    }

                    names[itm] += ")";
                    totalPrice *= itm.Qty;

                    if (cartItm.Constraint.Discount.IsPercent)
                    {
                        totalPrice *= (100 - cartItm.Constraint.Discount.Value) / 100;
                    }
                    else
                    {
                        totalPrice -= cartItm.Constraint.Discount.Value;
                    }

                    prices.Add(itm, totalPrice.ToString());
                    total += totalPrice;
                }
            }

            var vouchers = new List<int>();
            if (HttpContext.Request.Cookies["Vouchers"] is HttpCookie cookie)
                vouchers = JsonConvert.DeserializeObject<List<int>>(cookie.Value);

            foreach (var voucherId in vouchers)
            {
                var voucher = db.Vouchers
                        .Include(m => m.VoucherItems)
                        .FirstOrDefault(m => m.Id == voucherId);

                var flags = new List<bool>();
                foreach (var voucherItm in voucher.VoucherItems)
                {
                    var isIn = false;
                    foreach (var cartItm in cart.CartItems)
                    {
                        if (cartItm.StockId == voucherItm.StockId &&
                                cartItm.Qty >= voucherItm.Quantity)
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (isIn)
                        flags.Add(true);
                    else
                        flags.Add(false);
                }

                var allIn = true;
                foreach (var itm in flags)
                    if (!itm) allIn = false;

                if (allIn)
                {
                    total -= (voucher.IsPercent) ?
                        (total * (voucher.Value / 100)) : voucher.Value;

                    ViewBag.Vouchers += voucher.Name + " ";

                }
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            else
            {
                ViewBag.ReturnUrl = "";
            }

            return View(new CartIndexViewModel
            {
                Cart = cart,
                TotalCost = total,
                Prices = prices,
                ProductNames = names
            });
        }

        public ActionResult ContinueShopping(string returnUrl = "")
        {
            if (returnUrl != "")
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", controllerName: "Market");
        }

        private Cart GetCart(bool fromUpdate = false)
        {
            Cart cart;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var ecomuser = db.EcomUsers
                    .FirstOrDefault(m => m.ApplicationUserId == userId);
                // Get Cart and update it
                cart = db.Carts
                    .Include(m => m.CartItems)
                    .FirstOrDefault(m => m.EcomUserId == ecomuser.Id);

                if (cart is null)
                {
                    cart = db.Carts.Add(new Cart
                    {
                        EcomUserId = ecomuser.Id,
                        CartItems = new List<CartItem>()
                    });
                    db.SaveChanges();
                }

                if (cart.CartItems == null) cart.CartItems = new List<CartItem>();

                foreach (var itm in cart.CartItems)
                {
                    itm.Stock = db.Stocks
                                    .Include(m => m.Product)
                                    .FirstOrDefault(m => m.Id == itm.StockId);
                }

                if (!fromUpdate) UpdateCart(ref cart);
            }
            else
            {
                // Get guest Cart or create it if it doesn't exist
                if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
                    cart = JsonConvert.DeserializeObject<Cart>(cookie.Value);
                else
                {
                    cart = new Cart() { CartItems = new List<CartItem>() };
                    var cartCookie = JsonConvert.SerializeObject(cart);
                    cookie = new HttpCookie("guestCart");
                    cookie.Expires = DateTime.Now.AddMinutes(60);
                    cookie.Value = cartCookie;
                    Response.Cookies.Add(cookie);
                }

                if (cart.CartItems == null) cart.CartItems = new List<CartItem>();
            }
            return cart;
        }

        private int AddToCart(CartAddViewModel model, bool fromUpdate = false)
        {
            var cart = GetCart(fromUpdate);
            CartItem newCartItem = null;

            var stock = db.Stocks.FirstOrDefault(m => m.Id == model.ItemId);
            if (stock != null)
            {
                var isPresent = false;
                foreach (CartItem itm in cart.CartItems)
                {
                    if (itm.StockId == model.ItemId)
                    {
                        isPresent = true;
                        if (stock.MaxAmtPerUser >= (itm.Qty + model.Qty))
                        {
                            cart.CartItems.First(m => m.Id == itm.Id).Qty += model.Qty;
                        }
                        else
                            return -2;
                    }
                }

                if (!isPresent)
                {
                    newCartItem = new CartItem
                    {
                        StockId = model.ItemId,
                        Qty = model.Qty,
                        DiscountConstraintId = null,
                        IsEditable = true
                    };

                    cart.CartItems.Add(newCartItem);
                }
            }
            else
            {
                if (model.Type is null) return -5;

                var constraint = db.DiscountConstraints
                                    .FirstOrDefault(m => m.Id == model.ItemId
                                        && m.Type == model.Type);
                if (constraint is null) return -6;

                switch (constraint.Type)
                {
                    case DiscountConstraintType.Bundle:
                        {
                            var isPresent = false;
                            foreach (CartItem itm in cart.CartItems)
                            {
                                if (itm.StockId == null
                                    && itm.DiscountConstraintId == constraint.Id)
                                {
                                    isPresent = true;
                                    if (constraint.MaxAmt >= (itm.Qty + model.Qty))
                                    {
                                        cart.CartItems.First(m => m.Id == itm.Id).Qty += model.Qty;
                                    }
                                    else
                                        return -2;
                                }
                            }

                            if (!isPresent)
                            {
                                newCartItem = new CartItem
                                {
                                    StockId = null,
                                    Qty = model.Qty,
                                    DiscountConstraintId = constraint.Id,
                                    IsEditable = true
                                };

                                if (!User.Identity.IsAuthenticated)
                                    newCartItem.Id = DateTime.Now.GetHashCode();

                                cart.CartItems.Add(newCartItem);
                            }

                            break;
                        }

                    default:
                        {
                            return -7;
                        }
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
                {
                    var cartCookie = JsonConvert.SerializeObject(cart);
                    cookie.Value = cartCookie;
                    cookie.Expires = DateTime.Now.AddMinutes(60);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    return -3;
                }
            }

            return 0;
        }

        private int UpdateCart(ref Cart cart)
        {
            if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
            {
                var guestCart = JsonConvert.DeserializeObject<Cart>(cookie.Value);
                foreach (var itm in guestCart.CartItems)
                {
                    if (itm.StockId != null && itm.DiscountConstraintId == null)
                    {
                        AddToCart(
                            new CartAddViewModel
                            {
                                ItemId = (int)itm.StockId,
                                Qty = itm.Qty,
                                Type = null
                            },
                            true
                        );
                    }
                    else if (itm.StockId == null && itm.DiscountConstraintId != null)
                    {
                        AddToCart(
                            new CartAddViewModel
                            {
                                ItemId = (int)itm.DiscountConstraintId,
                                Qty = itm.Qty,
                                Type = db.DiscountConstraints
                                          .FirstOrDefault(m => m.Id ==
                                               itm.DiscountConstraintId).Type
                            },
                            true
                        );
                    }
                    else
                    {
                        return -1;
                    }
                }
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return 0;
        }
    }
}
