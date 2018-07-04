using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (User.Identity.IsAuthenticated && User.IsInRole("Vendor")) return RedirectToAction("Index", controllerName: "Vendor");
            return View();
        }

        public ActionResult Register()
        {
            return View(new HomeRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HomeRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vendor = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile.ToString(),
                    PrimaryRole = "Vendor",
                    VendorDetails = new Utilities.VendorDetails
                    {
                        BusinessName = model.BusinessName,
                        Zip = model.Zip,
                        State = model.State,
                        WebsiteUrl = model.WebsiteUrl
                    }
                };
                var result = UserManager.Create(vendor, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(vendor.Id, "Vendor");
                    SignInManager.SignIn(vendor, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", controllerName: "Vendor");
                }
                AddErrors(result);
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
    }
}