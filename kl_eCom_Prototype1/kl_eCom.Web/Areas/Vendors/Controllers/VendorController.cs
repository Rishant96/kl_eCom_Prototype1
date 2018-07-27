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
                .Include(m => m.Constraint)
                .Include(m => m.DiscountedItems)
                .Where(m => m.ApplicationUserId == vendorId)
                .ToList();

            var model = new VendorDiscountsViewModel {
                Ids = new List<int>(),
                Names = new Dictionary<int, string>(),
                DiscountIds = new Dictionary<int, int>(),
                DiscountValues = new Dictionary<int, string>(),
                StoreNames = new Dictionary<int, string>(),
                DiscountTypes = new Dictionary<int, string>(),
                StartDates = new Dictionary<int, string>(),
                ValidityPeriods = new Dictionary<int, string>()
            };

            var Names = new List<string>();
            var Values = new Dictionary<string, string>();
            var Stores = new Dictionary<string, string>();
            var Types = new Dictionary<string, string>();
            var Dates = new Dictionary<string, string>();
            var ValidityPeriods = new Dictionary<string, string>();
            var DiscountIds = new Dictionary<string, int>();

            foreach(var discount in discounts)
            {
                Names.Add(discount.Name);

                Stores.Add(discount.Name, discount.Store.Name);

                DiscountIds.Add(discount.Name, discount.Id);

                if (discount.IsPercent)
                    Values.Add(discount.Name, discount.Value + "%");
                else
                    Values.Add(discount.Name, (discount.Store.DefaultCurrencyType ?? "Rs.") 
                        + discount.Value);

                Types.Add(discount.Name, discount.Constraint.Type.ToString());

                Dates.Add(discount.Name, discount.StartDate
                        .ToUniversalTime().ToLongDateString());

                ValidityPeriods.Add(discount.Name, discount.ValidityPeriod + " Days");
            }

            var i = 1;
            foreach (var nameItm in Names)
            {
                foreach(var type in Types[nameItm])
                {
                    model.Ids.Add(i);
                    model.Names.Add(i, nameItm);
                    model.StartDates.Add(i, Dates[nameItm]);
                    model.ValidityPeriods.Add(i, ValidityPeriods[nameItm]);
                    model.StoreNames.Add(i, Stores[nameItm]);
                    model.DiscountValues.Add(i, Values[nameItm]);
                    model.DiscountTypes.Add(i, Types[nameItm]);
                    model.DiscountIds.Add(i, DiscountIds[nameItm]);
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
                StartDate = date
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

        public ActionResult DiscountDetails()
        {
            return View();
        }
    }
}