using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Entities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using kl_eCom.Web.Utilities;
using System.Web.ModelBinding;
using kl_eCom.Web.Infrastructure;

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
            var model = new ShopIndexViewModel { IsLeafDict = new Dictionary<Entities.Category, bool>(),
                                Breadcrum = new Dictionary<string, int>() };
            var store = db.Stores.FirstOrDefault(m => m.Id == id);
            if (store == null) return View("Error");
            model.Breadcrum.Add(store.Name, store.Id);
            if (catId == null)
            {
                model.Categories = db.Categories.Where(m => m.StoreId == id && m.IsBase == true).ToList();
            }
            else
            {
                var parentCat = db.Categories.FirstOrDefault(m => m.Id == catId);
                if (parentCat == null) return View("Error");
                model.Breadcrum.Add(parentCat.Name, parentCat.Id);
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
            ViewBag.Vendor = store.ApplicationUserId;
            return View(model);
        }

        public ActionResult Products(int? storeId, int? catId,
                                [Form] QueryOptions queryOptions, bool flag = false)
        {
            if (queryOptions == null) queryOptions = new QueryOptions();
            if(storeId == null || catId == null) return RedirectToAction("Index", "Market");
            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            ViewBag.Flag = flag;
            var store = db.Stores.FirstOrDefault(m => m.Id == storeId);
            if (store == null) return View("Error");
            var parent = db.Categories.FirstOrDefault(m => m.Id == catId);
            if (parent == null) return View("Error");
            var parentList = new Dictionary<string, int>();
            while (parent != null)
            {
                parentList.Add(parent.Name, parent.Id);
                parent = db.Categories.FirstOrDefault(m => m.Id == parent.CategoryId);
            }
            var catProdIds = db.Products.Where(m => m.CategoryId == catId).Select(m => m.Id).ToList();
            var model = new ShopProductsViewModel
            {
                Stocks = db.Stocks
                            .Include(m => m.Product)
                            .Where(m => m.StoreId == storeId && catProdIds.Contains(m.ProductId))
                            .OrderBy(queryOptions.Sort) 
                            .ToList(),
                Max = new Dictionary<int, int>(),
                Breadcrum = new Dictionary<string, int>(),
                SelectedOption = queryOptions.SortOption
            };

            model.Breadcrum.Add(store.Name, store.Id);

            var keys = parentList.Keys.ToList();
            keys.Reverse();
            foreach(var key in keys)
            {
                model.Breadcrum.Add(key, parentList[key]);
            }

            var cart = GetCart();

            foreach (var stk in model.Stocks)
            {
                var available = 0;
                if (stk.MaxAmtPerUser < stk.CurrentStock)
                {
                    available = stk.MaxAmtPerUser;
                }
                else
                {
                    available = stk.CurrentStock;
                }
                var cartItm = cart.CartItems.FirstOrDefault(m => m.StockId == stk.Id);
                if (cartItm != null)
                    model.Max.Add(stk.Id, available - cartItm.Qty);
                else
                    model.Max.Add(stk.Id, available);
            } 
            return View(model);
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
                return RedirectToAction("Products", new { storeId = storeID, catId = catID, flag = true });
            }

            TempData["storeId"] = storeID;
            TempData["catId"] = catID;
            return View(model);
        }
        
        [HttpPost]
        public ActionResult FilterSortProducts()
        {
            int? storeID = TempData["storeId"] as int?;
            int? catID = TempData["catId"] as int?;
            if (storeID == null || catID == null)
                return RedirectToAction("Index", "Market");
            var formOption = Request.Form["SortOption"];
            SortOption sortOption = (SortOption)Enum.Parse(typeof(SortOption), formOption);

            return RedirectToAction("Products", 
                new {
                    SortOption = sortOption,
                    storeId = storeID,
                    catId = catID
                });
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
                        ApplicationUserId = userId,
                        CartItems = new List<CartItem>()
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