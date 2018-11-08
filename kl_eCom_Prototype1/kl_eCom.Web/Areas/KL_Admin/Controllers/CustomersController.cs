using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.KL_Admin.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

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

        // GET: KL_Admin/Customers
        public ActionResult Index()
        {
            return View(new AdminCustomersIndexViewModel
            {
                Customers = db.EcomUsers
                            .Include(m => m.User)
                            .Where(m => m.PrimaryRole == "Customer")
                            .ToList()
            });
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return View("Error");
            return View(new AdminCustomersDetailsViewModel {
                EcomUser = db.EcomUsers
                           .Include(m => m.User)
                           .FirstOrDefault(m => m.Id == id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(AdminCustomersDetailsViewModel model)
        {
            if (model.EcomUser.Id != 0)
            {
                var ecomUser = db.EcomUsers.FirstOrDefault(m => m.Id == model.EcomUser.Id);
                ecomUser.IsActive = model.EcomUser.IsActive;
                db.Entry(ecomUser).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("Details", new { id = ecomUser.Id });
            }

            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(int? id)
        {
            if (id == null) return View("Error");
            var ecomUser = db.EcomUsers
                .Include(m => m.User)    
                .FirstOrDefault(m => m.Id == id);
            return View(new AdminCustomersResetPasswordViewModel {
                CustomerId = ecomUser.Id,
                Name = ecomUser.User.FirstName + " " + ecomUser.User.LastName,
                Email = ecomUser.User.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> ResetPasswordAsync(AdminCustomersResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.ConfirmPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return View("Error");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }

    public enum ManageMessageId
    {
        AddPhoneSuccess,
        ChangePasswordSuccess,
        SetTwoFactorSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess,
        RemovePhoneSuccess,
        Error
    }
}