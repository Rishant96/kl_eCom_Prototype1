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
        public bool isExternal = false;

        public ShopController()
        {
            // Take care of external urls   
        }

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
            ViewBag.Vendor = db.EcomUsers
                               .FirstOrDefault(m => m.Id == store.EcomUserId)
                               .ApplicationUserId;

            return View(model);
        }

        public ActionResult KL_Categories_Partial(List<KL_Category> categories)
        {
            return PartialView(new ShopKlCategoryViewModel {
                KL_Categories = categories
            });
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

            return PartialView(prodModel);
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
            return RedirectToAction("Products", new { storeId = (model.CategoryId == null) ? 
                                                                null as int?    : 
                                                                db.Categories
                                                                  .FirstOrDefault(m => m.Id == model.CategoryId)
                                                                  .StoreId,
                                                                catId = model.CategoryId,
                options.Price_MinValue, options.Price_MaxValue,
                options.Rating_Min, options.Allowed_Days,
                options.Availability,
                selectedOptions.PriceFilterSelected, 
                selectedOptions.RatingFilterSelected,
                selectedOptions.NewArrivalFilterSelected,
                selectedOptions.AvailabilityFilterSelected,
                SortOption = model.SelectedOption
            });
        }

        public ActionResult Products(int? storeId, int? catId, [Form] ShopFilteringOptions filteringOptions,
                                [Form] SelectedFilters selectedFilters,
                                [Form] QueryOptions queryOptions,
                                string searchQuery = null, bool flag = false, bool isKLId = false)
        {
            if (queryOptions == null) queryOptions = new QueryOptions();
           
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

            Category parent = null;
            Dictionary<string, int> parentList = new Dictionary<string, int>();
            List<int> catProdIds = null;
            List<KL_Category> klCategories = new List<KL_Category>();

            if (isKLId)
            {
                List<Category> categories = null;

                klCategories = db.KL_Categories
                                .Where(m => m.KL_CategoryId == catId)
                                .ToList();

                var parentCat = db.KL_Categories.FirstOrDefault(m => m.Id == catId);

                var klCatQueue = new Queue<KL_Category>();
                klCatQueue.Enqueue(parentCat);

                catProdIds = new List<int>();
                while (klCatQueue.Count > 0 && klCatQueue.Dequeue() is KL_Category kl_category)
                {
                    categories = db.Categories
                                   .Where(m => m.KL_CategoryId == kl_category.Id)
                                   .ToList();

                    foreach (var cat in categories)
                    {
                        var stocks = db.Stocks
                            .Include(m => m.Product)
                            .Where(m => m.Product.CategoryId == cat.Id)
                            .Select(m => m.Id)
                            .ToList();

                        catProdIds = catProdIds.Concat(stocks).ToList();
                    }

                    foreach (var kl_cat in db.KL_Categories
                        .Where(m => m.KL_CategoryId == kl_category.Id)
                        .ToList())
                    {
                        klCatQueue.Enqueue(kl_cat);
                    }
                }
                if (catProdIds.Count == 0)
                    ViewBag.EmptyMessage = "No products available";
            }
            else
            {
                parent = db.Categories.FirstOrDefault(m => m.Id == (catId ?? 0));
                while (parent != null)
                {
                    parentList.Add(parent.Name, parent.Id);
                    parent = db.Categories.FirstOrDefault(m => m.Id == parent.CategoryId);
                }
                catProdIds = db.Products.Where(m => m.CategoryId == (catId ?? m.CategoryId))
                                            .Select(m => m.Id).ToList();
            }

            var store = db.Stores.FirstOrDefault(m => m.Id == (storeId ?? 0));
            
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

            SearchParam search = null;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var stocks = db.Stocks
                             .Include(m => m.Product)
                             .Where(m => m.Product.Name.Contains(searchQuery))
                             .OrderBy(m => m.Product.Name)
                             .Select(m => m.Id)
                             .Distinct()
                             .ToArray();

                var categories = db.Categories
                                 .Where(m => m.Name.Contains(searchQuery))
                                 .OrderBy(m => m.Name)
                                 .Select(m => m.Id)
                                 .Distinct()
                                 .ToArray();
                
                TempData["Search"] = searchQuery;

                search = new SearchParam {
                    Categories = categories,
                    Stocks = stocks
                };
            }

            var model = new ShopProductsViewModel
            {
                CategoryId = catId,
                IsKlCat = isKLId,
                KL_Categories = klCategories,
                Stocks = (search == null || (search.Categories == null && search.Stocks == null)) ?
                          db.Stocks
                            .Include(m => m.Product)
                            .Where(m => m.StoreId == (storeId ?? m.StoreId)
                                && catProdIds.Contains(m.ProductId)
                                && m.Price >= filteringOptions.Price_MinValue 
                                && m.Price <= filteringOptions.Price_MaxValue
                                && m.Product.Rating >= filteringOptions.Rating_Min 
                                && m.Product.IsActive == true
                                && (filteringOptions.Availability || m.Status == StockStatus.InStock))
                            .OrderBy(queryOptions.Sort) 
                            .ToList()
                         :
                           null,
                Max = new Dictionary<int, int>(),
                Breadcrum = (store != null && parent != null) ? new Dictionary<string, int>()
                                                                : null,
                SelectedOption = queryOptions.SortOption,
                StoreId = storeId,
                FilteringOptions = filteringOptions
            };

            if (search != null)
            {
                if (search.Categories != null && search.Categories.Count() > 0)
                {
                    model.Stocks = db.Stocks
                                    .Include(m => m.Product)
                                    .Where(m => search.Categories.Contains(m.Product.CategoryId)
                                        && m.Price >= filteringOptions.Price_MinValue
                                        && m.Price <= filteringOptions.Price_MaxValue
                                        && m.Product.Rating >= filteringOptions.Rating_Min
                                        && m.Product.IsActive == true
                                        && (filteringOptions.Availability || m.Status == StockStatus.InStock))
                                    .OrderBy(queryOptions.Sort)
                                    .ToList();
                }
            
                if (search.Stocks != null && search.Stocks.Count() > 0)
                {
                    model.Stocks.Concat(db.Stocks
                        .Include(m => m.Product)
                        .Where(m => search.Stocks.Contains(m.Id)
                            && m.Price >= filteringOptions.Price_MinValue
                            && m.Price <= filteringOptions.Price_MaxValue
                            && m.Product.Rating >= filteringOptions.Rating_Min
                            && m.Product.IsActive == true
                            && (filteringOptions.Availability || m.Status == StockStatus.InStock))
                        .OrderBy(queryOptions.Sort)
                        .ToList());
                }
            }

            var stockList = model.Stocks ?? new List<Stock>();
            foreach (var stock in stockList)
            {
                if (!dateFlag && (stock.StockingDate - thresholdDate).Days < 0)
                {
                    model.Stocks.Remove(stock);
                }
            }
            if (model.Breadcrum != null)
            {
                model.Breadcrum.Add(store.Name, store.Id);

                var keys = parentList.Keys.ToList();
                keys.Reverse();
                foreach (var key in keys)
                {
                    model.Breadcrum.Add(key, parentList[key]);
                }
            }

            var cart = GetCart();

            foreach (var stk in model.Stocks ?? new List<Stock>())
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
                var cartItm = cart.CartItems
                        .FirstOrDefault(m => m.StockId == stk.Id && m.IsEditable);
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
            model.FilterViewModel.MinValue = filteringOptions.Price_MinValue;
            model.FilterViewModel.MaxValue = filteringOptions.Price_MaxValue;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Products(ShopProductsViewModel model, int? stockId)
        {
            int? storeID = TempData["storeId"] as int?;
            int? catID = TempData["catId"] as int?;

            if(model.Qty == 0 || ModelState.IsValid)
            {
                if (model.Qty == 0) model.Qty = 1;
                AddToCart(new CartAddViewModel { ItemId = (int)stockId, Qty = model.Qty });
                return RedirectToAction("Products", new { storeId = storeID, catId = catID, flag = true });
            }

            TempData["storeId"] = storeID;
            TempData["catId"] = catID;
            return View(model);
        }
        
        [HttpPost]
        public ActionResult FilterSortProducts([Form] ShopFilteringOptions filteringOptions,
                                [Form] SelectedFilters selectedFilters)
        {
            int? storeID = TempData["storeId"] as int?;
            int? catID = TempData["catId"] as int?;
            var formOption = Request.Form["SortOption"];
            SortOption sortOption = (SortOption)Enum.Parse(typeof(SortOption), formOption);

            return RedirectToAction("Products", 
                new {
                    SortOption = sortOption,
                    storeId = storeID,
                    catId = catID,
                    filteringOptions.Price_MinValue,
                    filteringOptions.Price_MaxValue,
                    filteringOptions.Rating_Min,
                    filteringOptions.Allowed_Days,
                    filteringOptions.Availability,
                    selectedFilters.PriceFilterSelected,
                    selectedFilters.RatingFilterSelected,
                    selectedFilters.NewArrivalFilterSelected,
                    selectedFilters.AvailabilityFilterSelected
                });
        } 

        public ActionResult ProductDetails(int? id, string returnUrl)
        {
            if (id == null) return View("Error");

            if (string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = "#";
            else
                ViewBag.ReturnUrl = returnUrl;

            var stock = db.Stocks
                .Include(m =>m.Store)
                .Include(m => m.Product)
                .Include(m => m.Product.Category)
                .Include(m => m.Product.ProductImages)
                .Include(m => m.Product.Specifications)
                .Include(m => m.Product.Category.Attributes)
                .FirstOrDefault(m => m.Id == id);
            if (stock == null) return View("Error");

            var cart = GetCart();
            var cartItms = cart.CartItems
                        .FirstOrDefault(m => m.StockId == id && m.IsEditable);
            var count = (cartItms != null) ? cartItms.Qty : 0;

            var discountIds = db.DiscountedItems
                                .Include(m => m.Discount)
                                .Where(m => m.StockId == id && m.Discount.IsActive
                                    && m.Discount.StartDate < DateTime.Now 
                                    && (!m.Discount.IsExpirable 
                                        || m.Discount.EndDate > DateTime.Now))
                                .Select(m => m.DiscountId)
                                .ToList();

            var discountConstraints = db.DiscountConstraints
                                        .Include(m => m.Discount)
                                        .Include(m => m.Discount.DiscountedItems)
                                        .Include(m => m.Discount.Store)
                                        .Where(m => discountIds
                                            .Contains(m.DiscountId)
                                            && m.Discount.IsActive)
                                        .ToList();

            var model = new ShopProductDetailsViewModel
            {
                Description = stock.Product.Category.Description,
                Stock = stock,
                StockId = stock.Id,
                ReturnUrl = ViewBag.ReturnUrl,
                AlreadyInCart = count,
                BundleDiscounts = new List<DiscountConstraint>(),
                BundleStocks = new Dictionary<DiscountConstraint, List<Stock>>(),
                BundleOldPrices = new Dictionary<DiscountConstraint, string>(),
                BundleNewPrices = new Dictionary<DiscountConstraint, string>(),
                MinOrderDiscounts = new List<DiscountConstraint>(),
                MinQtyDiscounts = new List<DiscountConstraint>(),
                NewPrice = null
            };

            foreach (var constraint in discountConstraints)
            {
                if (constraint.Type == DiscountConstraintType.Simple)
                {
                    if (constraint.Discount.IsPercent)
                    {
                        model.NewPrice = model.Stock.Price * 
                                    ((100 - constraint.Discount.Value)/100);
                    }
                    else
                    {
                        model.NewPrice = model.Stock.Price -
                                    constraint.Discount.Value;
                    }
                }
                else if (constraint.Type == DiscountConstraintType.MinOrder)
                {
                    model.MinOrderDiscounts.Add(constraint);
                }
                else if (constraint.Type == DiscountConstraintType.Qty)
                {
                    model.MinQtyDiscounts.Add(constraint);
                }
                else
                {
                    model.BundleDiscounts.Add(constraint);
                    model.BundleStocks.Add(constraint,
                        db.Stocks
                            .Where(m => db.BundledItems
                                    .FirstOrDefault(b => b.StockId == m.Id
                                        && b.DiscountConstraintId == constraint.Id)
                                    != null)
                            .ToList()
                    );

                    var total = 0.0f;
                    var curr = "";
                    foreach (var stk in model.BundleStocks[constraint])
                    {
                        total += stk.Price;
                        curr = stk.Store.DefaultCurrencyType;
                    }
                    model.BundleOldPrices.Add(constraint,
                        curr + " " + total);
                    if (constraint.Discount.IsPercent)
                    {
                        model.BundleNewPrices.Add(constraint,
                            curr + " " + (total * ((100 - constraint.Discount.Value) 
                                           / 100)));
                    }
                    else
                    {
                        model.BundleNewPrices.Add(constraint,
                            curr + " " + (total - constraint.Discount.Value));
                    }
                }
            }

            model.MinQtyDiscounts = model.MinQtyDiscounts.OrderBy(m => m.MinQty)
                                        .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDetails(ShopProductDetailsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            AddToCart(new CartAddViewModel {
                Qty = model.Qty,
                ItemId = model.StockId
            });

            return RedirectToAction("Index", "Cart", new { returnUrl = model.ReturnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BundleOrder(int? id, string returnUrl = "")
        {
            if (id != null)
            {
                var bundle = db.DiscountConstraints
                                .FirstOrDefault(m => m.Id == id
                                    && m.Type == DiscountConstraintType.Bundle);
                if (bundle == null) return View("Error");

                AddToCart(new CartAddViewModel {
                    Qty = 1,
                    ItemId = bundle.Id,
                    Type = bundle.Type
                });

                return RedirectToAction("Index", "Cart", new { returnUrl });
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkOrder(int? id, int? stockId, string returnUrl = "")
        {
            if (id != null || stockId == null)
            {
                var bulkDiscount = db.DiscountConstraints
                                .FirstOrDefault(m => m.Id == id
                                    && m.Type == DiscountConstraintType.Qty);
                if (bulkDiscount == null) return View("Error");

                AddToCart(new CartAddViewModel
                {
                    Qty = (int)bulkDiscount.MinQty,
                    ItemId = (int)stockId,
                    Type = bulkDiscount.Type
                });

                return RedirectToAction("Index", "Cart", new { returnUrl });
            }

            return RedirectToAction("Index", "Cart", new { returnUrl });
        }


        private Cart GetCart(bool fromUpdate = false)
        {
            Cart cart;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = db.EcomUsers
                    .FirstOrDefault(m => m.ApplicationUserId == userId);
                // Get Cart and update it
                cart = db.Carts
                    .Include(m => m.CartItems)
                    .FirstOrDefault(m => m.EcomUserId == user.Id);

                if (cart is null)
                {
                    cart = db.Carts.Add(new Cart
                    {
                        EcomUserId = user.Id,
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
                    cookie = new HttpCookie("guestCart")
                    {
                        Expires = DateTime.Now.AddMinutes(60),
                        Value = cartCookie
                    };
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
                    if (itm.StockId == model.ItemId && itm.IsEditable)
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
                        IsEditable = (model.Type == DiscountConstraintType.Qty) ?
                                        false : true
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

        public ActionResult ByProducts(int? id)
        {
            if (id == null)
            {
                var model = new ShopByProductsViewModel
                {
                    Categories = db.KL_Categories
                                   .Where(m => m.KL_CategoryId == null)
                                   .ToList()
                };

                return View(model);
            }
            else
            {
                if (db.KL_Categories
                      .FirstOrDefault(m => m.Id == id)
                        != null)
                {
                    return RedirectToAction("Products", new { catId = id, isKLId = true });
                }
                return View("Error");
            }
        }
    }
}