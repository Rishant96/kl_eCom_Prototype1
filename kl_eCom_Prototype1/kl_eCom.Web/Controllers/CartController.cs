using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index(bool flag = false, string returnUrl = "", bool checkoutErr = false)
        {
            var cart = GetCart();
            var total = 0.0f;
            var prices = new Dictionary<CartItem, string>();
            var names = new Dictionary<CartItem, string>();
            ViewBag.Flag = flag;
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

                        names[itm] +=  bndl.Stock.Product.Name;

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

            return View(new CartIndexViewModel {
                Cart = cart,
                TotalCost = total,
                Prices = prices,
                ProductNames = names
            });
        }

        public ActionResult Delete(int? id, string return_Url)
        {
            if (id == null) return RedirectToAction("Index");
            else
            {
                var cart = GetCart();
                CartItem cartItm = null;
                if (User.Identity.IsAuthenticated)
                {
                    cartItm = db.CartItems.FirstOrDefault(m => m.Id == id);
                    
                    var entry2 = db.Entry(cartItm);
                    if (entry2.State == EntityState.Detached)
                        db.CartItems.Attach(cartItm);
                    db.CartItems.Remove(cartItm);
                    db.SaveChanges();
                }
                else
                {
                    cartItm = cart.CartItems.FirstOrDefault(m => m.Id == id);

                    cart.CartItems.Remove(cartItm);
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
                }
                return RedirectToAction("Index", new { flag = true, returnUrl = return_Url });
            }
        }

        [Authorize(Roles = "Customer, Vendor")]
        public ActionResult Checkout(bool addrErr = false)
        {
            ViewBag.AddrErr = addrErr;
            var usrId = User.Identity.GetUserId();
            var cart = GetCart();
            if (cart.CartItems.Count == 0)
            {
                return RedirectToAction("Index", new { checkoutErr = true });
            }
            var addrs = db.Addresses
                        .Where(m => m.ApplicationUserId == usrId)
                        .ToList();
            var totalPrice = 0.0f;
            var cartItems = new List<CartItem>();
            foreach (var itm in cart.CartItems)
            {
                cartItems.Add(db.CartItems
                    .Include(m => m.Constraint)
                    .Include(m => m.Constraint.Discount)
                    .Include(m => m.Stock)
                    .FirstOrDefault(m => m.Id == itm.Id));
            }
            var prices = new Dictionary<int, float>();
            foreach (var itm in cartItems)
            {
                var miniTotal = 0.0f;
                if (itm.Stock != null)
                {
                    miniTotal = itm.Qty * itm.Stock.Price;
                }
                else
                {
                    var bundle = db.DiscountConstraints
                                        .Include(m => m.Discount)
                                        .Include(m => m.BundledItems)
                                        .FirstOrDefault(m => m.Id 
                                            == itm.DiscountConstraintId);

                    foreach (var bundleItm in bundle.BundledItems)
                    {
                        var stock = db.Stocks.FirstOrDefault(
                                        m => m.Id == bundleItm.StockId);

                        miniTotal += stock.Price;
                    }
                    miniTotal *= itm.Qty;
                    if (bundle.Discount.IsPercent)
                        miniTotal *= (100 - bundle.Discount.Value) / 100;
                    else
                        miniTotal -= bundle.Discount.Value;
                }
                prices.Add(itm.Id, miniTotal);
                totalPrice += miniTotal;
            }

            return View(new CheckoutViewModel {
                Addresses = addrs,
                CartItems = cartItems,
                CustomerName = User.Identity.GetUserName(),
                TotalPrice = totalPrice,
                Prices = prices
            });
        }


        [Authorize(Roles = "Customer, Vendor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var usrId = User.Identity.GetUserId();
            var addrId = int.Parse(Request.Form["SelectedAddress"] ?? "0");
            if (addrId != 0)
            {
                var addr = db.Addresses
                    .Include(m => m.User)
                    .FirstOrDefault(m => m.Id == addrId
                    && m.ApplicationUserId == usrId);
                if (addr != null)
                {
                    var cart = GetCart();

                    var hash = DateTime.Now.GetHashCode();
                    hash = (hash > 0) ? hash : -hash;

                    var ecomUser = db.EcomUsers
                        .FirstOrDefault(m => m.ApplicationUserId == usrId);

                    var order = db.Orders.Add(new Order
                    {
                        EcomUserId = ecomUser.Id,
                        OrderDate = DateTime.Now,
                        TotalCost = 0.0f,
                        OrderNumber = hash,
                        AddressId = addrId
                    });
                    db.SaveChanges();

                    var orderPerVendor = new Dictionary<int, List<int>>();

                    string subj = "Order Confirmation for Order #" + order.OrderNumber;
                    string msg = "Auto-genrated mail confirming the order you just placed via Khushlife E-Com,\n\n";
                    msg += "Order Details:\n";

                    foreach (var itm in cart.CartItems)
                    {
                        var vendorId = 0;
                        CartItem orderItem;
                        OrderItem dbOrderItm;

                        if (itm.StockId != null)
                        {
                            vendorId = db.EcomUsers
                                        .FirstOrDefault(m => m.Id 
                                            == itm.Stock.Store.EcomUserId)
                                        .Id;

                            orderItem = db.CartItems
                                        .Include(m => m.Stock)
                                        .Include(m => m.Stock.Store)
                                        .Include(m => m.Stock.Product)
                                        .FirstOrDefault(m => m.Id == itm.Id);
                            
                            dbOrderItm = db.OrderItems.Add(new OrderItem
                            {
                                OrderId = order.Id,
                                Order = order,
                                Qty = itm.Qty,
                                Price = itm.Stock.Price,
                                ProductName = itm.Stock.Product.Name,
                                StockId = (int)itm.StockId,
                                FinalCost = itm.Qty * itm.Stock.Price,
                                EcomUserId = vendorId,
                                Status = OrderStatus.NewOrder
                            });
                        }
                        else
                        {
                            vendorId = db.EcomUsers.FirstOrDefault(m => m.Id == 
                                        (db.DiscountConstraints
                                          .Include(n => n.Discount)
                                          .Include(n => n.Discount.Store)
                                          .FirstOrDefault(n => n.Id == itm.DiscountConstraintId))
                                          .Discount.Store.EcomUserId).Id;

                            orderItem = db.CartItems
                                        .Include(m => m.Constraint)
                                        .Include(m => m.Constraint.Discount)
                                        .Include(m => m.Constraint.Discount.Store)
                                        .Include(m => m.Constraint.BundledItems)
                                        .FirstOrDefault(m => m.Id == itm.Id);

                            var price = 0.0f;
                            foreach (var bundleItm in itm.Constraint.BundledItems)
                            {
                                if (bundleItm.Stock == null || bundleItm.Stock.Product == null)
                                    bundleItm.Stock = db.Stocks
                                        .Include(m => m.Product)
                                        .FirstOrDefault(m => m.Id == bundleItm.StockId);
                                price += bundleItm.Stock.Price;
                            }

                            if (itm.Constraint.Discount.IsPercent)
                                price *= (100 - itm.Constraint.Discount.Value) / 100;
                            else
                                price -= itm.Constraint.Discount.Value;

                            dbOrderItm = db.OrderItems.Add(new OrderItem
                            {
                                OrderId = order.Id,
                                Order = order,
                                Qty = itm.Qty,
                                Price = price,
                                ProductName = itm.Constraint.Discount.Name,
                                DiscountConstraintId = (int)itm.DiscountConstraintId,
                                FinalCost = itm.Qty * price,
                                EcomUserId = vendorId,
                                Status = OrderStatus.NewOrder
                            });
                        }

                        // here

                        // stock.CurrentStock -= itm.Qty;

                        order.TotalCost += dbOrderItm.FinalCost;
                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();

                        msg += "\tProduct: " + dbOrderItm.ProductName + ", Quantity: " + dbOrderItm.Qty +
                                ", Value = Rs. " + dbOrderItm.FinalCost + " (" + dbOrderItm.Price + " x " 
                                + dbOrderItm.Qty + ")\n";


                        if (db.Refferals
                              .Include(m => m.Customer)
                              .Include(m => m.Vendor)
                              .FirstOrDefault(
                                  m => m.Customer.ApplicationUserId == usrId 
                                  && m.Vendor.Id == vendorId)
                            is Refferal refferal)
                        {
                            if (refferal.IsBuyer == false)
                            {
                                refferal.IsBuyer = true;
                                refferal.DateBuyerAdded = DateTime.Now;
                                db.Entry(refferal).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == usrId);
                            var vendor = db.EcomUsers.FirstOrDefault(m => m.Id == vendorId);
                            db.Refferals.Add(new Refferal
                            {
                                CustomerId = user.Id,
                                VendorId = vendor.Id,
                                DateBuyerAdded = DateTime.Now,
                                IsBuyer = true,
                                IsRegisteredUser = false,
                                DateOfRegistration = null
                            });
                        }
                        db.SaveChanges();

                        if (orderPerVendor.Keys.Contains(vendorId))
                        {
                            orderPerVendor[vendorId].Add(dbOrderItm.Id);
                        }
                        else
                        {
                            orderPerVendor.Add(vendorId, new List<int>());
                            orderPerVendor[vendorId].Add(dbOrderItm.Id);
                        }
                    }

                    msg += "\nTotal Cost = Rs. " + order.TotalCost + "\n";
                    msg += "\nRegards,\nKhushlife E-Com Team";
                    FireEmail(db.Users.FirstOrDefault(m => m.Id == usrId).Email,
                        subj, msg);

                    foreach (var vndrId in orderPerVendor.Keys.ToList())
                    {
                        var vendor = db.EcomUsers
                                       .Include(m => m.User)
                                       .FirstOrDefault(m => m.Id == vndrId);
                        var customer = db.Users.FirstOrDefault(m => m.Id == usrId);
                        var klEmail = "khushlifeecommerce@gmail.com";
                        var klPass = "klEcom1234";
                        using (MailMessage mm = new MailMessage(klEmail, vendor.User.Email))
                        {
                            mm.Subject = "New Order Recieved: #" + order.OrderNumber;
                            mm.Body = "Order Details:\n";
                            mm.Body += "Order Date: " + order.OrderDate.ToLongDateString() + "\n";
                            mm.Body += ", Customer Name: " + customer.FirstName + " " + customer.LastName;
                            mm.Body += ", Contact Info: " + customer.PhoneNumber + "\n\n";

                            var i = 0;
                            foreach(var orderItmId in orderPerVendor[vendor.Id])
                            {
                                var orderItm = db.OrderItems
                                    .FirstOrDefault(m => m.Id == orderItmId);

                                mm.Body += "\tItem #" + ++i + ": Product Name - " + orderItm.ProductName +
                                    ", Quantity - " + orderItm.Qty + ", Cost - Rs. " + orderItm.FinalCost + "\n";
                            }
                            mm.Body += "\nTotal Cost = Rs. " + order.TotalCost + "\n";
                            mm.Body += "\n\nRegards,\nKhushlife E-Com";
                            mm.IsBodyHtml = false;
                            using (SmtpClient smtp = new SmtpClient())
                            {
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                NetworkCredential NetworkCred = new NetworkCredential(klEmail, klPass);
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = 587;
                                smtp.Send(mm);
                            }
                        }
                    }

                    var cartItems = cart.CartItems.ToList();
                    foreach (var cartItm in cartItems)
                    {
                        db.Entry(cartItm).State = EntityState.Deleted;
                    }
                    db.SaveChanges();
                    return RedirectToAction("MyOrders", "Customer");
                }
                else
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Checkout", new { addrErr = true });
        }

        private void FireEmail(string to, string subject, string message, bool isBodyHtml = false)
        {
            var klEmail = "khushlifeecommerce@gmail.com";
            var klPass = "klEcom1234";
            using (MailMessage mm = new MailMessage(klEmail, to))
            {
                mm.Subject = subject;
                mm.Body = message;
                mm.IsBodyHtml = isBodyHtml;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(klEmail, klPass);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }

        public ActionResult VoucherPartial()
        {
            return PartialView("VoucherPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoucherPartial(CartVoucherViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.VoucherName = model.VoucherName.ToUpper();
                var voucher = db.Vouchers
                    .Include(m => m.VoucherItems)
                    .FirstOrDefault(m =>
                        m.IsActive && m.StartDate < DateTime.Now &&
                        (!m.IsExpirable || m.EndDate > DateTime.Now) &&
                        m.Name == model.VoucherName);
                
                var vouchers = new List<int>();
                if (HttpContext.Request.Cookies["Vouchers"] is HttpCookie cookie)
                    vouchers = JsonConvert.DeserializeObject<List<int>>(cookie.Value);
                
                if ( voucher.MaxAvailPerCustomer > vouchers
                     .Where(m => m == voucher.Id)
                     .ToList()
                     .Count)
                    vouchers.Add(voucher.Id);

                cookie = new HttpCookie("Vouchers")
                {
                    Expires = DateTime.Now.AddMinutes(60),
                    Value = JsonConvert.SerializeObject(vouchers)
                };
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ContinueShopping(string returnUrl = "")
        {
            if(returnUrl != "")
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
                    itm.Stock = db.Stocks.Include(m => m.Product).FirstOrDefault(m => m.Id == itm.StockId);
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