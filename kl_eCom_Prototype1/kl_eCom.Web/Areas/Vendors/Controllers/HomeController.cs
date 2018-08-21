using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    public class HomeController : Controller
    {
        #region CTOR + Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        #endregion

        // GET: Vendors/Home
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Vendor"))
                return RedirectToAction("Index", controllerName: "Vendor");
            return View();
        }

        public ActionResult Register()
        {
            var db = new ApplicationDbContext();
            var availablePackages = db.VendorPlans
                     .Where(m => m.IsActive == true)
                     .ToList();
            return View(new HomeRegisterViewModel
            {
            //    AvailablePackages = availablePackages,
            //    VendorPackageSelected = (availablePackages
            //        .FirstOrDefault(m => m.Price == 0.0f)).Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HomeRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // model.VendorPackageSelected = int.Parse(Request.Form["PlansList"]);
                var db = new ApplicationDbContext();
                // var plan = db.VendorPackages.FirstOrDefault(m => m.Id == model.VendorPackageSelected);

                var vendor = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile,
                    PrimaryRole = "Vendor",
                    VendorDetails = new Utilities.VendorDetails
                    {
                        BusinessName = model.BusinessName,
                        Zip = model.Zip,
                        State = model.State,
                        WebsiteUrl = model.WebsiteUrl,
                        RegistrationDate = DateTime.Now
                    }
                };
                var result = UserManager.Create(vendor, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(vendor.Id, "Vendor");
                    try
                    {
                        var plan = db.VendorPlans
                                    .FirstOrDefault(m => m.Price == 0.0f
                                        && m.IsActive == true
                                        && m.IsEnabled == true);
                        db.ActivePlans.Add(new Utilities.ActivePlan
                        {
                            ApplicationUserId = vendor.Id,
                            VendorPlanId = plan.Id,
                            VendorPlanPaymentDetailId = null,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddYears(1),
                            PaymentStatus = true
                        });
                        db.SaveChanges();

                        vendor.VendorDetails.ActivePlanId = db.ActivePlans.FirstOrDefault(m => m.ApplicationUserId == vendor.Id).Id;
                        db.Entry(vendor.VendorDetails).State = System.Data.Entity.EntityState.Modified;
                        db.Entry(vendor).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        SignInManager.SignIn(vendor, isPersistent: false, rememberBrowser: false);

                        return RedirectToAction("Index", controllerName: "Vendor");
                    }
                    catch (Exception)
                    {
                        db.Entry(vendor.VendorDetails).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        UserManager.Delete(vendor);
                        return View("Error");
                    }
                }
                AddErrors(result);
            }
            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}