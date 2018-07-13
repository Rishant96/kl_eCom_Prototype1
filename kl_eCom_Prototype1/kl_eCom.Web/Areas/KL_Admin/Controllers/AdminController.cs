using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: KL_Admin/Admin
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName == "klAdmin" 
                        && model.Email == "khushlife@gmail.com"
                        && model.Password == "KLadmin@123")
                {
                    var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

                    var result = signInManager.PasswordSignIn(model.UserName, model.Password, false, false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                        {
                            return RedirectToAction("Home");
                        }
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            return RedirectToAction("Index");
        }



        public ActionResult Home()
        {
            var model = new AdminHomeViewModel
            {
                VendorPackages = db.VendorPackages.ToList()
            };
            return View(model);
        }

        public ActionResult Plans()
        {
            return View(new AdminPlansIndexViewModel {
                VendorPackages = db.VendorPackages.ToList()
            });
        }
    }
}