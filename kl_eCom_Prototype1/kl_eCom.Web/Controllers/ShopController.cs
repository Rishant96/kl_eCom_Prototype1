using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Entities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using kl_eCom.Web.Utilities;

namespace kl_eCom.Web.Controllers
{
    public class ShopController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Shop
        public ActionResult Index(int? id, int? catId)
        {
            if (id == null) return RedirectToAction("Index", "Market");
            ViewBag.storeId = id;
            var model = new ShopIndexViewModel { IsLeafDict = new Dictionary<Entities.Category, bool>() };
            if (catId == null)
            {
                model.Categories = db.Categories.Where(m => m.StoreId == id && m.IsBase == true).ToList();
            }
            else
            {
                model.Categories = db.Categories.Where(m => m.CategoryId == catId).ToList();
            }
            foreach (var cat in model.Categories)
            {
                if (db.Categories.Where(m => m.CategoryId == cat.Id).ToList().Count == 0)
                {
                    model.IsLeafDict.Add(cat, true);
                }
                else
                {
                    model.IsLeafDict.Add(cat, false);
                }
            }
            return View(model);
        }

        public ActionResult Products(int? storeId, int? catId)
        {
            if(storeId == null || catId == null) return RedirectToAction("Index", "Market");
            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            var catProdIds = db.Products.Where(m => m.CategoryId == catId).Select(m => m.Id).ToList();
            return View(new ShopProductsViewModel
            {
                Stocks = db.Stocks
                            .Include(m => m.Product)
                            .Where(m => m.StoreId == storeId && catProdIds.Contains(m.Id))
                            .ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Products(ShopProductsViewModel model, int? stockId)
        {
            int? storeID = TempData["storeId"] as int?;
            int? catID = TempData["catId"] as int?;
            if (storeID == null || catID == null || stockId == null)
                return RedirectToAction("Index", "Market");

            if(ModelState.IsValid)
            {
                AddToCart(new CartAddViewModel { StockId = (int)stockId, Qty = model.Qty });
                return RedirectToAction("Products", new { storeId = storeID, catId = catID });
            }

            TempData["storeId"] = storeID;
            TempData["catId"] = catID;
            return View(model);
        }

        private Cart GetCart(bool fromUpdate = false)
        {
            Cart cart;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                // Get Cart and update it
                cart = db.Carts.Include(m => m.CartItems).FirstOrDefault(m => m.ApplicationUserId == userId);
                if (cart is null)
                {
                    cart = db.Carts.Add(new Cart
                    {
                        ApplicationUserId = userId
                    });
                    db.SaveChanges();
                }
                if(!fromUpdate) UpdateCart(ref cart);
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
            if (cart.CartItems == null) cart.CartItems = new List<CartItem>();
            foreach (CartItem itm in cart.CartItems)
            {
                if (itm.StockId == model.StockId)
                {
                    isPresent = true;
                    if((itm.Qty + model.Qty) <= stock.CurrentStock)
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