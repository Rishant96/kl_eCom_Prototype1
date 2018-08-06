using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using kl_eCom.Web.Areas.Vendors.Models;
using Microsoft.AspNet.Identity.Owin;
using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{ 
    [Authorize(Roles = "Vendor")]
    public class VendorController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public VendorController()
        {
        }

        public VendorController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Vendors/Vendor
        public ActionResult Index()
        {
            var vendorId = User.Identity.GetUserId();
            var vendor = db.Users
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.Id == vendorId);
            

            var pkg = db.VendorPlans
                        .FirstOrDefault(m => m.Id ==
                        vendor.VendorDetails.ActivePlan.VendorPlanId);

            var prods = db.Products
                        .Include(m => m.Category)
                        .Include(m => m.Category.Store)
                        .Where(m => m.Category.Store.ApplicationUserId == vendor.Id)
                        .ToList();

            var prodCount = prods.Count;

            if (pkg.MaxProducts < prodCount)
                ViewBag.Flag = true;
            else
                ViewBag.Flag = false;

            ViewBag.CurrProds = prods.Count;
            ViewBag.MaxProds = pkg.MaxProducts;

            return View();
        }

        public ActionResult Details()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Users
                .Include(m => m.VendorDetails)
                .FirstOrDefault(m => m.Id == userId)
            );
        }

        public ActionResult Edit()
        {
            // Encrypt UserId when editing
            var userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(m => m.VendorDetails)
                .FirstOrDefault(m => m.Id == userId);
            return View(new VendorEditViewModel {
                BusinessName = user.VendorDetails.BusinessName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Mobile = user.PhoneNumber,
                WebsiteUrl = user.VendorDetails.WebsiteUrl,
                Zip = user.VendorDetails.Zip,
                State = user.VendorDetails.State
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorEditViewModel model)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users
                    .Include(m => m.VendorDetails)
                    .FirstOrDefault(m => m.Id == userId);

                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.LastName;
                user.UserName = model.UserName;
                user.VendorDetails.BusinessName = model.BusinessName;
                user.VendorDetails.WebsiteUrl = model.WebsiteUrl;
                user.VendorDetails.Zip = model.Zip;
                user.VendorDetails.State = model.State;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            var usrId = User.Identity.GetUserId();
            var usr = db.Users.FirstOrDefault(m => m.Id == usrId);
            return View(new VendorPasswordChangeViewModel {
                UserName = usr.UserName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> ChangePasswordAsync(VendorPasswordChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Details");
                }
                AddErrors(result);
                return View(model);
            }
            return View(model);
        }

        public ActionResult Plan()
        {
            var userId = User.Identity.GetUserId();
            var activePlan = db.ActivePlans
                                .Include(m => m.PaymentDetails)
                                .FirstOrDefault(m => m.ApplicationUserId 
                                    == userId);
            return View(new VendorPlanIndexViewModel {
                UserName = User.Identity.GetUserName(),
                CurrentPackage = db.VendorPlans
                    .FirstOrDefault(m => m.Id == activePlan.VendorPlanId),
                PaymentDetails = activePlan.PaymentDetails
            });
        }

        public ActionResult ChangePlan()
        {
            var userId = User.Identity.GetUserId();
            var activePlan = db.ActivePlans
                                .Include(m => m.PaymentDetails)
                                .FirstOrDefault(m => m.ApplicationUserId
                                    == userId);

            return View(new VendorPlanChangeViewModel {
                UserName = User.Identity.GetUserName(),
                CurrentPackage = db.VendorPlans
                    .FirstOrDefault(m => m.Id == activePlan.VendorPlanId),
                Packages = db.VendorPlans
                    .Where(m => m.Id != activePlan.VendorPlanId && m.IsEnabled == true)
                    .Select(m => m.DisplayName)
                    .ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePlan(VendorPlanChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pkg = db.VendorPlans.FirstOrDefault(m => m.DisplayName == model.SelectedPackage);
                if (pkg == null) return View("Error");
                var usrId = User.Identity.GetUserId();
                
                db.PlanChangeRequests.Add(new Utilities.PlanChangeRequest {
                    ApplicationUserId = usrId,
                    VendorPlanId = pkg.Id,
                    Status = Utilities.RequestStatus.Pending,
                    RequestDate = DateTime.Now
                });
                db.SaveChanges();
                // raise alert
                return RedirectToAction("Details");
            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult Discounts()
        {
            var vendorId = User.Identity.GetUserId();

            var discounts = db.Discounts
                .Include(m => m.Store)
                .Include(m => m.DiscountedItems)
                .Where(m => m.ApplicationUserId == vendorId)
                .ToList();

            var activeDiscounts = discounts
                                    .Where(m => m.IsActive)
                                    .OrderByDescending(m => m.StartDate)
                                    .ToList();

            var inactiveDiscounts = discounts
                                    .Where(m => !m.IsActive)
                                    .OrderByDescending(m => m.StartDate)
                                    .ToList();

            discounts = activeDiscounts.Concat(inactiveDiscounts).ToList();

            var model = new VendorDiscountsViewModel {
                Ids = new List<int>(),
                Names = new Dictionary<int, string>(),
                DiscountIds = new Dictionary<int, int>(),
                DiscountValues = new Dictionary<int, string>(),
                StoreNames = new Dictionary<int, string>(),
                DiscountTypes = new Dictionary<int, List<string>>(),
                StartDates = new Dictionary<int, string>(),
                ValidityPeriods = new Dictionary<int, string>(),
                Products = new Dictionary<int, List<string>>(),
                Statuses = new Dictionary<int, string>()
            };

            var Names = new List<string>();
            var DiscountStatuses = new Dictionary<string, string>();
            var Values = new Dictionary<string, string>();
            var Stores = new Dictionary<string, string>();
            var Types = new Dictionary<string, List<string>>();
            var Dates = new Dictionary<string, string>();
            var ValidityPeriods = new Dictionary<string, string>();
            var DiscountIds = new Dictionary<string, int>();
            var ProductsList = new Dictionary<string, List<string>>();
            
            foreach(var discount in discounts)
            {
                Names.Add(discount.Name);

                Stores.Add(discount.Name, discount.Store.Name);

                DiscountIds.Add(discount.Name, discount.Id);

                DiscountStatuses.Add(discount.Name, (discount.IsActive) ? 
                                "Active" : "In-active");

                ProductsList.Add(discount.Name, new List<string>());
                foreach(var item in discount.DiscountedItems)
                {
                    var prodId = db.Stocks.FirstOrDefault(
                               m => m.Id == item.StockId).ProductId;

                    ProductsList[discount.Name].Add(
                        db.Products.FirstOrDefault(
                               m => m.Id == prodId)
                               .Name);
                }

                if (discount.IsPercent)
                    Values.Add(discount.Name, discount.Value + "%");
                else
                    Values.Add(discount.Name, (discount.Store.DefaultCurrencyType ?? "Rs.") 
                        + discount.Value);

                var constraints = db.DiscountConstraints.Where(
                    m => m.DiscountId == discount.Id).ToList();
                if (constraints != null)
                {
                    Types.Add(discount.Name, new List<string>());

                    var simpleConst = constraints.FirstOrDefault(m => m.Type
                        == Utilities.DiscountConstraintType.Simple);
                    if (simpleConst != null)
                    {
                        Types[discount.Name].Add("Simple");
                    }

                    var minOrderConst = constraints.FirstOrDefault(m => m.Type
                        == Utilities.DiscountConstraintType.MinOrder);
                    if (minOrderConst != null)
                    {
                        Types[discount.Name].Add("Minimum Order");
                    }

                    var bundleConst = constraints.FirstOrDefault(m => m.Type
                        == Utilities.DiscountConstraintType.Bundle);
                    if (bundleConst != null)
                    {
                        Types[discount.Name].Add("Bundle");
                    }

                    var bulkConst = constraints.FirstOrDefault(m => m.Type
                        == Utilities.DiscountConstraintType.Qty);
                    if (bulkConst != null)
                    {
                        Types[discount.Name].Add("Bulk");
                    }
                }
                else
                    Types.Add(discount.Name, new List<string> { "No constraints" });
                
                Dates.Add(discount.Name, discount.StartDate
                        .ToUniversalTime().ToLongDateString());

                if (discount.ValidityPeriod != null)
                {
                    ValidityPeriods.Add(discount.Name,
                        discount.ValidityPeriod + " Days");
                }
                else
                {
                    ValidityPeriods.Add(discount.Name,
                        "\t-");
                }
            }

            var i = 1;
            foreach (var nameItm in Names)
            {
                model.Ids.Add(i);
                model.Names.Add(i, nameItm);
                model.StartDates.Add(i, Dates[nameItm]);
                model.Statuses.Add(i, DiscountStatuses[nameItm]);
                model.ValidityPeriods.Add(i, ValidityPeriods[nameItm]);
                model.StoreNames.Add(i, Stores[nameItm]);
                model.DiscountValues.Add(i, Values[nameItm]);
                model.DiscountIds.Add(i, DiscountIds[nameItm]);
                model.DiscountTypes.Add(i, Types[nameItm]);
                model.Products.Add(i, new List<string>());
                foreach (var prod in ProductsList[nameItm])
                {
                    model.Products[i].Add(prod);
                }
                i++;
            }

            return View(model);
        }

        public ActionResult CreateDiscount()
        {
            var oldDate = DateTime.Now;
            var date = oldDate.AddDays(1);
            date = date.AddHours(-1 * oldDate.Hour);
            date = date.AddMinutes(-1 * oldDate.Minute);
            date = date.AddSeconds(-1 * oldDate.Second);

            var model = new VendorDiscountCreateViewModel {
                AvailableCategories = new List<Category>(),
                AvailableProducts = new Dictionary<Category, List<Product>>(),
                StartDate = date,
                IsActive = true
            };

            var vendorId = User.Identity.GetUserId();
            var stores = db.Stores
                        .Include(m => m.Categories)
                        .Where(m => m.ApplicationUserId == vendorId)
                        .ToList();

            foreach (var store in stores)
            {
                model.AvailableCategories = new List<Category>();
                foreach (var cat in store.Categories)
                {
                    var prods = db.Products
                                .Where(m => m.CategoryId == cat.Id 
                                && (db.Stocks.FirstOrDefault(n => 
                                        n.ProductId == m.Id) 
                                != null))
                                .ToList();

                    if (prods != null && prods.Count > 0)
                    {
                        model.AvailableCategories.Add(cat);
                        model.AvailableProducts.Add(cat, prods);
                    }
                }
            }

            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDiscount(VendorDiscountCreateViewModel model)
        {
            var vendorId = User.Identity.GetUserId();

            var prodsSlctd = Request.Form["Products"];
            if (string.IsNullOrEmpty(prodsSlctd))
                ModelState.AddModelError("", "No Products Selected");

            var type = Request.Form["Type"];
            if (string.IsNullOrEmpty(type))
                ModelState.AddModelError("", "No Discount Type Selected");

            var valueType = Request.Form["IsInPercent"];
            if (string.IsNullOrEmpty(valueType))
                ModelState.AddModelError("Model.Value", "Please select value type");

            if (model.ValidityPeriod == 0)
            {
                ModelState.AddModelError("Model.ValidityPeriod", "Invalid validity period entered");
            }

            if (model.Value == 0)
            {
                ModelState.AddModelError("Model.Value", "Invalid discount value entered");
            }

            if (ModelState.IsValid)
            {
                string[] prodsArr = prodsSlctd.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                List<int> prodIds = new List<int>();
                
                foreach (var pStr in prodsArr)
                    prodIds.Add(int.Parse(pStr));


                if (type == "3" && prodsArr.Count() < 2)
                {
                    ModelState.AddModelError("", "Cannot create a bundle with 1 product," 
                                + "\nplease use 'Simple' discount instead.");

                    model.AvailableCategories = new List<Category>();
                    model.AvailableProducts = new Dictionary<Category, List<Product>>();

                    var stores_2 = db.Stores
                                .Include(m => m.Categories)
                                .Where(m => m.ApplicationUserId == vendorId)
                                .ToList();

                    foreach (var store in stores_2)
                    {
                        model.AvailableCategories = new List<Category>();
                        foreach (var cat in store.Categories)
                        {
                            var prods = db.Products
                                        .Where(m => m.CategoryId == cat.Id)
                                        .ToList();
                            if (prods != null || prods.Count > 0)
                            {
                                model.AvailableCategories.Add(cat);
                                model.AvailableProducts.Add(cat, prods);
                            }
                        }
                    }

                    return View(model);
                }

                var discount = new Discount {
                        ApplicationUserId = vendorId,
                        Name = model.Name,
                        Description = model.Description,
                        IsActive = model.IsActive,
                        StartDate = model.StartDate,
                        IsExpirable = model.IsExpirable,
                        IsConstrained = false,
                        Value = model.Value,
                        ValidityPeriod = model.ValidityPeriod,
                        StoreId = db.Stores.FirstOrDefault(m => m.ApplicationUserId 
                                    == vendorId).Id,
                    };
                
                List<DiscountedItem> discountedItems = new List<DiscountedItem>();
                foreach (var prodId in prodIds)
                {
                    var stk = db.Stocks.FirstOrDefault(m =>
                                    m.ProductId == prodId);
                    discountedItems.Add(db.DiscountedItems.Add(
                        new Utilities.DiscountedItem {
                            DiscountId = discount.Id,
                            StockId = stk.Id
                        })
                    );
                }

                if (valueType == "1")
                {
                    discount.IsPercent = true;
                    if (discount.Value >= 100)
                    {
                        foreach (var itm in discountedItems)
                        {
                            db.DiscountedItems.Remove(itm);
                        }
                        db.SaveChanges();

                        db.Entry(discount).State = EntityState.Deleted;
                        db.SaveChanges();
                        return View("Error");
                    }
                }
                else if (valueType == "2")
                    discount.IsPercent = false;
                else
                {
                    ModelState.AddModelError("Model.Value", "Please select a value type");
                    return View(model);
                }

                discount.IsExpirable = model.IsExpirable;
                if (model.IsExpirable)
                {
                    discount.ValidityPeriod = model.ValidityPeriod;
                    discount.EndDate = model.StartDate.AddDays(model.ValidityPeriod ?? 0);
                }
                else
                {
                    discount.ValidityPeriod = null;
                    discount.EndDate = null;
                }
                
                discount = db.Discounts.Add(discount);
                db.SaveChanges();

                switch (type) {
                    case "1":
                        {
                            bool flag = false;
                            var dscntItms = new List<DiscountedItem>(discount.DiscountedItems);
                            foreach (var discountItem in dscntItms)
                            {
                                var stock = db.Stocks
                                            .FirstOrDefault(m => m.Id == discountItem.StockId);
                                if (!discount.IsPercent && stock.Price < discount.Value)
                                {
                                    db.Entry(discountItem).State = EntityState.Deleted;
                                    flag = true;
                                }
                            }
                            db.SaveChanges();

                            if (flag)
                            {
                                db.Entry(discount).State = EntityState.Deleted;
                                db.SaveChanges();
                                return View("Error");
                            }

                            db.Entry(discount).State = EntityState.Modified;
                            db.DiscountConstraints.Add(new Utilities.DiscountConstraint
                            {
                                Type = Utilities.DiscountConstraintType.Simple,
                                DiscountId = discount.Id
                            });
                            break;
                        }

                    case "2":
                        {
                            discount.IsConstrained = true;
                            db.Entry(discount).State = EntityState.Modified;
                            db.DiscountConstraints.Add(new Utilities.DiscountConstraint {
                                Type = Utilities.DiscountConstraintType.MinOrder,
                                MinOrder = float.Parse(model.ExtraInfo),
                                DiscountId = discount.Id
                            });
                            break;
                        }

                    case "3":
                        {
                            var price = 0.0f;
                            var dscntItms = new List<DiscountedItem>(discount.DiscountedItems);
                            foreach (var discountItem in dscntItms)
                            {
                                var stock = db.Stocks
                                            .FirstOrDefault(m => m.Id == discountItem.StockId);
                                price += stock.Price;
                            }
                            if (!discount.IsPercent && price <= discount.Value)
                            {
                                foreach (var discountItem in dscntItms)
                                {
                                    db.Entry(discountItem).State = EntityState.Deleted;
                                }

                                db.Entry(discount).State = EntityState.Deleted;
                                db.SaveChanges();
                                return View("Error");
                            }

                            db.SaveChanges();

                            discount.IsConstrained = true;
                            db.Entry(discount).State = EntityState.Modified;

                            var constraint = new Utilities.DiscountConstraint
                            {
                                Type = Utilities.DiscountConstraintType.Bundle,
                                DiscountId = discount.Id,
                                MaxAmt = int.Parse(model.ExtraInfo),
                                BundledItems = new List<BundledItem>()
                            };

                            foreach (var id in prodIds)
                            {
                                var stock = db.Stocks.FirstOrDefault(m => m.Id == id);
                                constraint.BundledItems.Add(new BundledItem {
                                    StockId = id
                                });
                            }

                            db.DiscountConstraints.Add(constraint);

                            break;
                        }

                    case "4":
                        {
                            discount.IsConstrained = true;
                            db.Entry(discount).State = EntityState.Modified;
                            db.DiscountConstraints.Add(new Utilities.DiscountConstraint
                            {
                                Type = Utilities.DiscountConstraintType.Qty,
                                MinQty = int.Parse(model.ExtraInfo),
                                DiscountId = discount.Id
                            });
                            break;
                        }

                    default:
                        {

                            break;
                        }
                }
                db.SaveChanges();

                return RedirectToAction("Discounts");
            }
            #region Errors
            model.AvailableCategories = new List<Category>();
            model.AvailableProducts = new Dictionary<Category, List<Product>>();

            var stores = db.Stores
                        .Include(m => m.Categories)
                        .Where(m => m.ApplicationUserId == vendorId)
                        .ToList();

            foreach (var store in stores)
            {
                model.AvailableCategories = new List<Category>();
                foreach (var cat in store.Categories)
                {
                    var prods = db.Products
                                .Where(m => m.CategoryId == cat.Id)
                                .ToList();
                    if (prods != null || prods.Count > 0)
                    {
                        model.AvailableCategories.Add(cat);
                        model.AvailableProducts.Add(cat, prods);
                    }
                }
            }

            return View(model);
            #endregion
        }

        public ActionResult DiscountDetails(int? id)
        {
            if (id == null) return View("Error");

            var discount = db.Discounts
                    .Include(m => m.DiscountedItems)
                    .FirstOrDefault(m => m.Id == id);
            if (discount == null) return View("Error");

            var model = new VendorDiscountDetailsViewModel {
                Discount = discount,
                Constraint = db.DiscountConstraints.FirstOrDefault(
                    m => m.DiscountId == discount.Id),
                Products = new List<Product>()
            };

            foreach (var itm in discount.DiscountedItems)
            {
                var stock = db.Stocks
                              .Include(m => m.Product)
                              .FirstOrDefault(m => m.Id
                                    == itm.StockId);

                model.Products.Add(stock.Product);
            }

            return View(model);
        }

        public ActionResult EditDiscount(int? id)
        {
            if (id == null) return View("Error");

            var discount = db.Discounts
                            .Include(m => m.DiscountedItems)
                            .Include(m => m.Store)
                            .FirstOrDefault(m => m.Id == id);
            if (discount == null) return View("Error");

            var constraint = db.DiscountConstraints
                        .FirstOrDefault(m => m.DiscountId == id);

            var stockIds = db.DiscountedItems
                            .Where(m => m.DiscountId == id)
                            .Select(m => m.StockId)
                            .ToList();

            var model = new VendorDiscountEditViewModel {
                Id = discount.Id,
                Name = discount.Name,
                Description = discount.Description,
                IsActive = discount.IsActive,
                IsExpirable = discount.IsExpirable,
                StartDate = discount.StartDate,
                ValidityPeriod = discount.ValidityPeriod,
                Value = discount.Value,
                Type = constraint.Type.ToString(),
                SelectedProducts = db.Stocks
                                    .Where(m => stockIds.Contains(m.Id))
                                    .Select(m => m.ProductId)
                                    .ToArray()
            };

            model.DiscountType = constraint.Type;

            switch (constraint.Type)
            {
                case Utilities.DiscountConstraintType.Simple:
                    {
                        model.ExtraInfo = "\t-";
                        break;
                    }

                case Utilities.DiscountConstraintType.MinOrder:
                    {
                        model.ExtraInfo = discount.Store.DefaultCurrencyType + constraint.MinOrder;
                        break;
                    }

                case Utilities.DiscountConstraintType.Qty:
                    {
                        model.ExtraInfo = constraint.MinQty + " items";
                        break;
                    }

                case Utilities.DiscountConstraintType.Bundle:
                    {
                        model.MaxAmt = (int)constraint.MaxAmt;
                        model.ExtraInfo = null;
                        break;
                    }
            }

            model.AvailableCategories = new List<Category>();
            model.AvailableProducts = new Dictionary<Category, List<Product>>();

            var vendorId = User.Identity.GetUserId();

            var stores = db.Stores
                        .Include(m => m.Categories)
                        .Where(m => m.ApplicationUserId == vendorId)
                        .ToList();

            foreach (var store in stores)
            {
                model.AvailableCategories = new List<Category>();
                foreach (var cat in store.Categories)
                {
                    var prods = db.Products
                                .Where(m => m.CategoryId == cat.Id)
                                .ToList();
                    if (prods != null && prods.Count > 0)
                    {
                        model.AvailableCategories.Add(cat);
                        model.AvailableProducts.Add(cat, prods);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDiscount(VendorDiscountEditViewModel model)
        {
            var prodsSlctd = Request.Form["Products"];
            if (string.IsNullOrEmpty(prodsSlctd))
                ModelState.AddModelError("", "No Products Selected");

            var valueType = Request.Form["IsInPercent"];
            if (string.IsNullOrEmpty(valueType))
                ModelState.AddModelError("Model.Value", "Please select value type");

            if (model.ValidityPeriod == 0)
            {
                ModelState.AddModelError("Model.ValidityPeriod", "Invalid validity period entered");
            }

            if (model.Value == 0)
            {
                ModelState.AddModelError("Model.Value", "Invalid discount value entered");
            }

            if (ModelState.IsValid)
            {
                string[] prodsArr = prodsSlctd.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                List<int> prodIds = new List<int>();

                if (model.DiscountType == DiscountConstraintType.Bundle)
                {
                    if (prodsArr.Count() <= 1)
                    {
                        return View("Error");
                    }
                }

                foreach (var pStr in prodsArr)
                    prodIds.Add(int.Parse(pStr));

                var discount = db.Discounts
                            .Include(m => m.DiscountedItems)
                            .FirstOrDefault(m => m.Id == model.Id);

                discount.Name = model.Name;
                discount.Description = model.Description;
                discount.IsActive = model.IsActive;
                discount.Value = model.Value;



                if (model.MaxAmt > 0)
                {
                    var constraint = db.DiscountConstraints.FirstOrDefault
                        (m => m.DiscountId == discount.Id
                        && m.Type == DiscountConstraintType.Bundle);

                    if(constraint != null)
                    {
                        constraint.MaxAmt = model.MaxAmt;
                        db.Entry(constraint).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                var items = new List<DiscountedItem>(discount.DiscountedItems);
                foreach (var itm in items)
                {
                    db.Entry(itm).State = EntityState.Deleted;
                }
                
                db.SaveChanges();

                foreach (var prodId in prodIds)
                {
                    db.DiscountedItems.Add(new Utilities.DiscountedItem
                    {
                        DiscountId = discount.Id,
                        StockId = db.Stocks.FirstOrDefault(m =>
                                    m.ProductId == prodId).Id
                    });
                }

                db.SaveChanges();

                if (valueType == "1")
                    discount.IsPercent = true;
                else if (valueType == "2")
                    discount.IsPercent = false;
                else
                {
                    ModelState.AddModelError("Model.Value", "Please select a value type");
                    return View(model);
                }

                discount.IsExpirable = model.IsExpirable;
                if (model.IsExpirable)
                {
                    discount.ValidityPeriod = model.ValidityPeriod;
                    discount.EndDate = model.StartDate.AddDays(model.ValidityPeriod ?? 0);
                }
                else
                {
                    discount.ValidityPeriod = null;
                    discount.EndDate = null;
                }

                db.Entry(discount).State = EntityState.Modified;              
                db.SaveChanges();

                return RedirectToAction("Discounts");
            }

            return View("Error");
        }

        public ActionResult StopDiscount(int? id)
        {
            if (id == null) return View("Error");

            var discount = db.Discounts.FirstOrDefault(m => m.Id == id);
            if (discount == null)
                return View("Error");
            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("StopDiscount")]
        public ActionResult StopDiscountPost(int? id)
        {
            if (id == null) return View("Error");
            var discount = db.Discounts.FirstOrDefault(
                        m => m.Id == id);
            if (discount == null) return View("Error");
            discount.IsActive = false;
            db.Entry(discount).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Discounts");
        }

        public ActionResult Vouchers()
        {
            var vendorId = User.Identity.GetUserId();

            var vouchers = db.Vouchers
                             .Include(m => m.Vendor)
                             .Include(m => m.VoucherItems)
                             .Where(m => m.ApplicationUserId == vendorId)
                             .ToList();

            var currencyType = "";
            var flag = true;
            foreach (var voucher in vouchers)
            {
                foreach (var voucherItem in voucher.VoucherItems)
                {
                    if (flag)
                    {
                        currencyType = db.Stocks
                                           .Include(m => m.Store)
                                           .FirstOrDefault(m => m.Id
                                                  == voucherItem.StockId)
                                           .Store
                                           .DefaultCurrencyType;
                        flag = false;
                    }
                    voucherItem.StockedProduct = db.Stocks
                                                   .Include(m => m.Product)
                                                   .FirstOrDefault(m => m.Id
                                                        == voucherItem.StockId);
                }
            }

            var redeemedVouchers = db.RedeemedVouchers
                                     .Include(m => m.Customer)
                                     .Include(m => m.Voucher)
                                     .Where(m => vouchers.Contains(m.Voucher))
                                     .ToList();

            var model = new VendorVouchersIndexViewModel {
                Vouchers = vouchers ?? new List<Voucher>(),
                RedeemedVouchers = redeemedVouchers ?? new List<RedeemedVoucher>(),
                DefaultCurrencyType = currencyType
            };

            return View(model);
        }

        public ActionResult CreateVoucher()
        {
            var oldDate = DateTime.Now;
            var date = oldDate.AddDays(1);
            date = date.AddHours(-1 * oldDate.Hour);
            date = date.AddMinutes(-1 * oldDate.Minute);
            date = date.AddSeconds(-1 * oldDate.Second);

            var model = new VendorVoucherCreateViewModel {
                StartDate = date,
                IsAutomatic = null,
                IsExpirable = true,
                IsLimited = true,
                Value = 10.0f,
                ItemPartials = new int[5] { 0, 0, 0, 0, 0 },
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVoucher(VendorVoucherCreateViewModel model)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("Vouchers");
            }

            return View(model);
        }
    }
}