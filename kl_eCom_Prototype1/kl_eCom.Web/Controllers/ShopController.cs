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
        
        public ActionResult FiltersPartial(ShopProductsViewModel prodModel)
        {
            prodModel.FilterViewModel.PriceSelection.PriceSelectionItems
                = new List<PriceSelectionItem>
                    {
                        new PriceSelectionItem
                        {
                            Id = 1,
                            DisplayName = "0 to 10,000",
                            MinPrice = 0,
                            MaxPrice = 10000
                        },
                        new PriceSelectionItem
                        {
                            Id = 2,
                            DisplayName = "10,000 to 20,000",
                            MinPrice = 10000,
                            MaxPrice = 20000
                        },
                        new PriceSelectionItem
                        {
                            Id = 3,
                            DisplayName = "Enter Price Range",
                            MinPrice = -1,
                            MaxPrice = -1
                        }
                    };

            prodModel.FilterViewModel.RatingSelection.RatingSelectionItems
                 = new List<RatingSelectionItem>
                {
                        new RatingSelectionItem
                        {
                            Id = 1,
                            DisplayName = "Average Rating: 5",
                            MinRating = 5
                        },
                        new RatingSelectionItem
                        {
                            Id = 2,
                            DisplayName = "Average Rating: 4 and above",
                            MinRating = 4
                        },
                        new RatingSelectionItem
                        {
                            Id = 3,
                            DisplayName = "Average Rating: 3 and above",
                            MinRating = 3
                        },
                        new RatingSelectionItem
                        {
                            Id = 4,
                            DisplayName = "Average Rating: 2 and above",
                            MinRating = 2
                        },
                        new RatingSelectionItem
                        {
                            Id = 5,
                            DisplayName = "Average Rating: 1 and above",
                            MinRating = 1
                        }
                };

            prodModel.FilterViewModel.NewestArrivalSelection.NewestArrivalSelectionItems
                = new List<NewestArrivalSelectionItem>
                {
                        new NewestArrivalSelectionItem
                        {
                            Id = 1,
                            DisplayName = "Last 30 days",
                            AllowedDays = 30
                        },
                        new NewestArrivalSelectionItem
                        {
                            Id = 2,
                            DisplayName = "Last 60 days",
                            AllowedDays = 60
                        },
                        new NewestArrivalSelectionItem
                        {
                            Id = 3,
                            DisplayName = "Older",
                            AllowedDays = int.MaxValue
                        }
                };

            prodModel.FilterViewModel.AvailabilitySelection.AvailabilitySelectionItems
                = new List<AvailabilitySelectionItem>
                    {
                        new AvailabilitySelectionItem
                        {
                            Id = 1,
                            DisplayName = "Only In Stock",
                            Value = false
                        },
                        new AvailabilitySelectionItem
                        {
                            Id = 2,
                            DisplayName = "All Products",
                            Value = true
                        }
                    };

            //if (prodModel.FilteringOptions != null && prodModel.FilterViewModel != null)
            //{
            //    if (prodModel.FilteringOptions.PriceOption != null)
            //        prodModel.FilterViewModel.PriceSelection.PriceItemSelected
            //            = prodModel.FilterViewModel.PriceSelection.PriceSelectionItems
            //                .FirstOrDefault(m => m.Id == prodModel.FilteringOptions.PriceOption);

            //    if (prodModel.FilteringOptions.RatingOption != null)
            //        prodModel.FilterViewModel.RatingSelection.RatingItemSelected
            //            = prodModel.FilterViewModel.RatingSelection.RatingSelectionItems
            //                .FirstOrDefault(m => m.Id == prodModel.FilteringOptions.RatingOption);

            //    if (prodModel.FilteringOptions.ArrivalOption != null)
            //        prodModel.FilterViewModel.AvailabilitySelection.AvailabilityItemSelected
            //            = prodModel.FilterViewModel.AvailabilitySelection.AvailabilitySelectionItems
            //                .FirstOrDefault(m => m.Id == prodModel.FilteringOptions.AvailabilityOption);

            //    if (prodModel.FilteringOptions.AvailabilityOption != null)
            //        prodModel.FilterViewModel.NewestArrivalSelection.NewestArrivalItemSelected
            //            = prodModel.FilterViewModel.NewestArrivalSelection.NewestArrivalSelectionItems
            //                .FirstOrDefault(m => m.Id == prodModel.FilteringOptions.ArrivalOption);
            //}

            return View(prodModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("FiltersPartial")]
        public ActionResult FiltersPartialPost(ShopProductsViewModel model)
        {
            var priceOption = Request.Form["PriceList"];
            var ratingOption = Request.Form["RatingList"];
            var newArrivalsOption = Request.Form["NewArrivalsList"];
            var availabilityOption = Request.Form["AvailabilityList"];

            var options = new ShopFilteringOptions();
            var selectedOptions = new SelectedFilters();

            if (priceOption != null)
            {
                switch (priceOption)
                {
                    case "1":
                    {
                        options.Price_MaxValue = 10000;
                        break;
                    }
                    case "2":
                    {
                        options.Price_MinValue = 10000;
                        options.Price_MaxValue = 20000;
                        break;
                    }
                    case "3":
                    {
                        if (model.FilterViewModel.MinValue > 0 && model.FilterViewModel.MaxValue > 0)
                        {
                            options.Price_MaxValue = model.FilterViewModel.MaxValue;
                            options.Price_MinValue = model.FilterViewModel.MinValue;
                        }
                        break;
                    }
                }
            }
            else
            {
                priceOption = "0";
            }

            if (ratingOption != null)
            {
                switch (ratingOption)
                {
                    case "1":
                    {
                        options.Rating_Min = 5;
                        break;
                    }
                    case "2":
                    {
                        options.Rating_Min = 4;
                        break;
                    }
                    case "3":
                    {
                        options.Rating_Min = 3;
                        break;
                    }
                    case "4":
                    {
                        options.Rating_Min = 2;
                        break;
                    }
                    case "5":
                    {
                        options.Rating_Min = 1;
                        break;
                    }
                }
            }
            else
            {
                ratingOption = "0";
            }

            if (newArrivalsOption != null)
            {
                switch (newArrivalsOption)
                {
                    case "1":
                        {
                            options.Allowed_Days = 30;
                            break;
                        }
                    case "2":
                        {
                            options.Allowed_Days = 60;
                            break;
                        }
                    case "3":
                        {
                            break;
                        }
                }
            }
            else
            {
                newArrivalsOption = "0";
            }

            if (availabilityOption != null)
            {
                switch (availabilityOption)
                {
                    case "1":
                    {
                            options.Availability = false;
                        break;
                    }
                    case "2":
                    {
                            options.Availability = true;
                        break;
                    }
                }
            }
            else
            {
                availabilityOption = "0";
            }
            selectedOptions.PriceFilterSelected = int.Parse(priceOption);
            selectedOptions.RatingFilterSelected = int.Parse(ratingOption);
            selectedOptions.NewArrivalFilterSelected = int.Parse(newArrivalsOption);
            selectedOptions.AvailabilityFilterSelected = int.Parse(availabilityOption);
            return RedirectToAction("Products", new { storeId = db.Categories.FirstOrDefault(m => m.Id == model.CategoryId).StoreId, catId = model.CategoryId,
                
                Price_MinValue = options.Price_MinValue, Price_MaxValue = options.Price_MaxValue, Rating_Min = options.Rating_Min,
                Allowed_Days = options.Allowed_Days, Availability = options.Availability,
                PriceFilterSelected = selectedOptions.PriceFilterSelected, 
                RatingFilterSelected = selectedOptions.RatingFilterSelected,
                NewArrivalFilterSelected = selectedOptions.NewArrivalFilterSelected,
                AvailabilityFilterSelected = selectedOptions.AvailabilityFilterSelected,
                SortOption = model.SelectedOption
            });
        }

        public ActionResult Products(int? storeId, int? catId, [Form] ShopFilteringOptions filteringOptions,
                                [Form] SelectedFilters selectedFilters,
                                [Form] QueryOptions queryOptions, bool flag = false)
        {
            if (queryOptions == null) queryOptions = new QueryOptions();
            if(storeId == null || catId == null) return RedirectToAction("Index", "Market");
            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            bool? recFlag = TempData["flag"] as bool?;
            if (recFlag != null && recFlag == true)
            {
                TempData["flag"] = null;
                return RedirectToAction("Products", new { storeId, catId, queryOptions });
            }
            else if(flag)
            {
                TempData["flag"] = flag;
            }
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
            var thresholdDate = DateTime.Now;
            bool dateFlag = false;
            if (filteringOptions.Allowed_Days == -1)
            {
                dateFlag = true;
            }
            else
            {
                thresholdDate = thresholdDate.AddDays(-1 * filteringOptions.Allowed_Days);
            }
            var model = new ShopProductsViewModel
            {
                CategoryId = (int)catId,
                Stocks = db.Stocks
                            .Include(m => m.Product)
                            .Where(m => m.StoreId == storeId 
                                && catProdIds.Contains(m.ProductId)
                                && m.Price >= filteringOptions.Price_MinValue 
                                && m.Price <= filteringOptions.Price_MaxValue
                                && m.Product.Rating >= filteringOptions.Rating_Min 
                                && m.Product.IsActive == true
                                && (filteringOptions.Availability || m.Status == StockStatus.InStock))
                            .OrderBy(queryOptions.Sort) 
                            .ToList(),
                Max = new Dictionary<int, int>(),
                Breadcrum = new Dictionary<string, int>(),
                SelectedOption = queryOptions.SortOption,
                StoreId = (int)storeId
            };
            var stockList = model.Stocks;
            foreach (var stock in stockList)
            {
                if (!dateFlag && (stock.StockingDate - thresholdDate).Days < 0)
                {
                    model.Stocks.Remove(stock);
                }
            }
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
            if (model.FilterViewModel == null)
            {
                model.FilterViewModel = new ShopFilterViewModel {
                    PriceSelection = new PriceSelection(),
                    RatingSelection = new RatingSelection(),
                    NewestArrivalSelection = new NewestArrivalSelection(),
                    AvailabilitySelection = new AvailabilitySelection()
                };
            }
            model.FilterViewModel.PriceSelection.PriceItemSelected = selectedFilters.PriceFilterSelected;
            model.FilterViewModel.RatingSelection.RatingItemSelected = selectedFilters.RatingFilterSelected;
            model.FilterViewModel.NewestArrivalSelection.NewestArrivalItemSelected = selectedFilters.NewArrivalFilterSelected;
            model.FilterViewModel.AvailabilitySelection.AvailabilityItemSelected = selectedFilters.AvailabilityFilterSelected;
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

            if(model.Qty == 0 || ModelState.IsValid)
            {
                if (model.Qty == 0) model.Qty = 1;
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