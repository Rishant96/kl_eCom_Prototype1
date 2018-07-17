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
                        .Include(m => m.VendorDetails.ActivePackage)
                        .FirstOrDefault(m => m.Id == vendorId);

            var pkg = db.VendorPackages
                        .FirstOrDefault(m => m.Id ==
                        vendor.VendorDetails.ActivePackage.VendorPackageId);

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
            var activePlan = db.ActivePackages
                                .Include(m => m.PaymentDetails)
                                .FirstOrDefault(m => m.ApplicationUserId 
                                    == userId);
            return View(new VendorPlanIndexViewModel {
                UserName = User.Identity.GetUserName(),
                CurrentPackage = db.VendorPackages
                    .FirstOrDefault(m => m.Id == activePlan.VendorPackageId),
                PaymentDetails = activePlan.PaymentDetails
            });
        }

        public ActionResult ChangePlan()
        {
            var userId = User.Identity.GetUserId();
            var activePlan = db.ActivePackages
                                .Include(m => m.PaymentDetails)
                                .FirstOrDefault(m => m.ApplicationUserId
                                    == userId);

            return View(new VendorPlanChangeViewModel {
                UserName = User.Identity.GetUserName(),
                CurrentPackage = db.VendorPackages
                    .FirstOrDefault(m => m.Id == activePlan.VendorPackageId),
                Packages = db.VendorPackages
                    .Where(m => m.Id != activePlan.VendorPackageId && m.IsEnabled == true)
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
                var pkg = db.VendorPackages.FirstOrDefault(m => m.DisplayName == model.SelectedPackage);
                if (pkg == null) return View("Error");
                var usrId = User.Identity.GetUserId();
                
                db.PlanChangeRequests.Add(new Utilities.PlanChangeRequest {
                    ApplicationUserId = usrId,
                    VendorPackageId = pkg.Id,
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
    }
}