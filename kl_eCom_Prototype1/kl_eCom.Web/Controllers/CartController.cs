using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = GetCart();
            var total = 0.0f;
            var prices = new Dictionary<CartItem, float>();
            var names = new Dictionary<CartItem, string>();
            foreach (var itm in cart.CartItems)
            {
                var stock = db.Stocks.FirstOrDefault(m => m.Id == itm.StockId);
                var cost = itm.Qty * stock.Price;
                var prod = db.Products.FirstOrDefault(m => m.Id == stock.ProductId);
                names.Add(itm, prod.Name);
                prices.Add(itm, cost);
                total += cost;
            }
            return View(new CartIndexViewModel {
                Cart = cart,
                TotalCost = total,
                Prices = prices,
                ProductNames = names
            });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            else
            {
                var cart = GetCart();
                var cartItm = db.CartItems.FirstOrDefault(m => m.Id == id);
                var entry = db.Entry(cartItm);
                if (entry.State == EntityState.Detached)
                    db.CartItems.Attach(cartItm);
                db.CartItems.Remove(cartItm);
                db.SaveChanges();

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
                return RedirectToAction("Index");
            }
        }

        public ActionResult Checkout()
        {
            return View();
        }

        private Cart GetCart(bool fromUpdate = false)
        {
            Cart cart;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                // Get Cart and update it
                cart = db.Carts
                    .Include(m => m.CartItems)
                    .FirstOrDefault(m => m.ApplicationUserId == userId);

                if (cart is null)
                {
                    cart = db.Carts.Add(new Cart
                    {
                        ApplicationUserId = userId
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
            }
            return cart;
        }

        private int AddToCart(CartAddViewModel model, bool fromUpdate = false)
        {
            var cart = GetCart(fromUpdate);

            var stock = db.Stocks.FirstOrDefault(m => m.Id == model.StockId);
            if (stock is null) return -1;

            var isPresent = false;
            foreach (CartItem itm in cart.CartItems)
            {
                if (itm.StockId == model.StockId)
                {
                    isPresent = true;
                    if ((itm.Qty + model.Qty) <= stock.CurrentStock)
                    {
                        cart.CartItems.First(m => m.Id == itm.Id).Qty += model.Qty;
                    }
                    else
                        return -2;
                }
            }

            if (!isPresent)
            {
                cart.CartItems.Add(new CartItem
                {
                    StockId = model.StockId,
                    Qty = model.Qty
                });
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
                    AddToCart(new CartAddViewModel { StockId = itm.StockId, Qty = itm.Qty }, true);
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