using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class VendorsController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
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
        
        // GET: KL_Admin/Vendors
        public ActionResult Index()
        {
            return View(new AdminVendorsIndexViewModel {
                Vendors = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.VendorDetails)
                            .Include(m => m.VendorDetails.ActivePlan)
                            .Where(m => m.PrimaryRole == "Vendor")
                            .ToList()
            });
        }

        public ActionResult Details(string id)
        {
            var vendor = db.EcomUsers
                            .Include(m => m.VendorDetails)
                            .Include(m => m.User)
                            .FirstOrDefault(m => m.ApplicationUserId == id);
            if (vendor == null || vendor.VendorDetails == null) return View("Error");

            return View(new AdminVendorsDetailsViewModel {
                Vendor = vendor,
                VendorDetails = vendor.VendorDetails,
                ActivePackage = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetail)
                                  .FirstOrDefault(m => m.EcomUserId == vendor.Id),
                DowngradeRequest = db.VendorDowngradeRecords
                                .FirstOrDefault(m => m.EcomUserId == vendor.Id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(AdminVendorsDetailsViewModel model)
        {
            if (model != null && model.Vendor != null && !string.IsNullOrEmpty(model.Vendor.ApplicationUserId))
            {
                var vendor = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == model.Vendor.ApplicationUserId);
                vendor.IsActive = model.Vendor.IsActive;
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id =  model.Vendor.ApplicationUserId});
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return View("Error");
            var model = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.ApplicationUserId)
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.ApplicationUserId == id);
            if (model is null) return View("Error");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(EcomUser model)
        {
            if (string.IsNullOrEmpty(model.ApplicationUserId)) return View("Error");
            var vendor = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.ApplicationUserId == model.ApplicationUserId);
            db.Entry(vendor.VendorDetails).State = System.Data.Entity.EntityState.Deleted;
            db.Users.Attach(vendor.User);
            db.Entry(vendor.User).State = System.Data.Entity.EntityState.Deleted;
            db.EcomUsers.Attach(vendor);
            db.Entry(vendor).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult EditDomain(string id)
        {
            var vendor = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.ApplicationUserId == id);
            if (vendor == null) return View("Error");
            return View(new AdminVendorsDomainEditViewModel {
                FullName = vendor.User.FirstName + " " + vendor.User.LastName,
                RegisterDate = vendor.VendorDetails.RegistrationDate,
                DomainDate = vendor.VendorDetails.DomainRegistrationDate
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDomain(AdminVendorsDomainEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vendor = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.VendorDetails)
                            .Include(m => m.VendorDetails.ActivePlan)
                            .Include(m => m.VendorDetails.ActivePlan.Plan)
                            .FirstOrDefault(m => m.ApplicationUserId == model.Id);
                vendor.VendorDetails.DomainRegistrationDate = model.DomainDate;

                var date = (DateTime)model.DomainDate;
                var daysNotToCharge = (vendor.VendorDetails.ActivePlan.EndDate -
                                          date.AddYears(DateTime.Now.Year - date.Year + 1))
                                          .Days;
                var validRefund = ((vendor.VendorDetails.ActivePlan.Plan.Price
                                    * (1 + (vendor.VendorDetails.ActivePlan.Plan.GST / 100)))
                                    / 365) * daysNotToCharge;

                db.VendorPlanChangeRecord.Add(new VendorPlanChangeRecord {
                    EcomUserId = vendor.Id,
                    Balance = vendor.VendorDetails.ActivePlan.Balance ?? 0.0f,
                    VendorPlanPaymentDetailId = null,
                    PlanName = vendor.VendorDetails.ActivePlan.Plan.DisplayName,
                    StartDate = vendor.VendorDetails.ActivePlan.StartDate,
                    TimeStamp = DateTime.Now,
                    VendorPlanId = vendor.VendorDetails.ActivePlan.Plan.Id,
                });

                vendor.VendorDetails.ActivePlan.Balance -= validRefund;
                vendor.VendorDetails.ActivePlan.EndDate = date.AddYears(DateTime.Now.Year - date.Year + 1);
                vendor.VendorDetails.ActivePlan.StartDate = DateTime.Now;
                
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }
        
        public ActionResult PlanChange(string id)
        {
            var vendor = db.EcomUsers
                        .Include(m => m.User)
                        .Include(m => m.VendorDetails)
                        .FirstOrDefault(m => m.ApplicationUserId == id);
            if (vendor == null || vendor.VendorDetails == null) return View("Error");
            var activePckg = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetail)
                                  .FirstOrDefault(m => m.EcomUserId 
                                        == vendor.Id);
            return View(new AdminVendorsPlanChangeViewModel {
                VendorName = vendor.User.FirstName + " " + vendor.User.LastName,
                VendorId = vendor.ApplicationUserId,
                ActivePlan = activePckg,
                Amount = activePckg.Balance ?? 0.0f,
                DowngradeRecord = db.VendorDowngradeRecords
                    .Include(m => m.NewPlan)
                    .FirstOrDefault(m => m.EcomUserId 
                            == vendor.Id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlanChange(AdminVendorsPlanChangeViewModel model, bool flag)
        {
            if (flag)
            {
                var ecomModel = db.EcomUsers
                    .FirstOrDefault(m => m.ApplicationUserId
                        == model.VendorId);
                var activePkg = db.ActivePlans
                        .Include(m => m.Plan)
                        .FirstOrDefault(m => m.EcomUserId == ecomModel.Id);

                var downgradeRecord = db.VendorDowngradeRecords
                                .Include(m => m.NewPlan)
                                .FirstOrDefault(m => m.EcomUserId == ecomModel.Id);

                var changeRecord = new VendorPlanChangeRecord
                {
                    Balance = activePkg.Balance ?? 0.0f,
                    PlanName = activePkg.Plan.DisplayName,
                    StartDate = activePkg.StartDate,
                    TimeStamp = DateTime.Now,
                    VendorPlanId = downgradeRecord.VendorPlanId,
                    VendorPlanPaymentDetailId = null
                };
                db.VendorPlanChangeRecord.Add(changeRecord);

                var balanceType = int.Parse(Request.Form["BalanceType"]);
                switch (balanceType)
                {
                    case 1:
                        {
                            activePkg.Balance = 0.0f;
                            activePkg.PaymentStatus = true;
                            break;
                        }
                    case 2:
                        {
                            var balance = ((activePkg.Plan.Price - downgradeRecord.NewPlan.Price) / 365)
                                            * (activePkg.EndDate - DateTime.Now).Days;
                            activePkg.Balance -= balance;
                            activePkg.Balance = (float)Math.Floor((float)activePkg.Balance);
                            if (activePkg.Balance >= -5 && activePkg.Balance <= 5)
                                activePkg.Balance = 0.0f;
                            if (activePkg.Balance <= 0.0f) activePkg.PaymentStatus = true;
                            break;
                        }
                    default:
                        {
                            return View("Error");
                        }
                }
                activePkg.VendorPlanId = downgradeRecord.VendorPlanId;

                db.Entry(activePkg).State = EntityState.Modified;
                db.Entry(downgradeRecord).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.VendorId });
            }

            var user = db.EcomUsers
                .FirstOrDefault(m => m.ApplicationUserId == model.VendorId);
            var mode = int.Parse(Request.Form["Mode"]);
            if (!string.IsNullOrEmpty(model.VendorId))
            {
                var activePkg = db.ActivePlans
                        .FirstOrDefault(m => m.EcomUserId == user.Id);
                activePkg.PaymentDetail = new VendorPlanPaymentDetail {
                    AmountPaid = model.Amount,
                    PaymentDate = DateTime.Now,
                    Notes = model.Notes
                };
                switch (mode)
                {
                    case 1:
                        {
                            activePkg.PaymentDetail.PaymentType = PaymentType.Cash;
                            break;
                        }
                    case 2:
                        {
                            activePkg.PaymentDetail.PaymentType = PaymentType.NetBanking;
                            break;
                        }
                    case 3:
                        {
                            activePkg.PaymentDetail.PaymentType = PaymentType.References;
                            break;
                        }
                    case 4:
                        {
                            activePkg.PaymentDetail.PaymentType = PaymentType.Other;
                            break;
                        }
                    default:
                        {
                            return View("Error");
                        }
                }
                activePkg.Balance -= model.Amount;
                activePkg.Balance =  (float)Math.Floor((float)activePkg.Balance);
                if (activePkg.Balance >= -5 && activePkg.Balance <= 5)
                    activePkg.Balance = 0.0f;
                activePkg.PaymentStatus = (activePkg.Balance == 0.0f) ? true : false;
                db.Entry(activePkg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.VendorId });
            }
            return View("Error");
        }


        public ActionResult FixActiveProducts()
        {
            ViewBag.Confirmation = true;
            return View();
        }

        [HttpPost]
        public ActionResult FixActiveProducts(string key)
        {
            ViewBag.Confirmation = false;

            if (key != "klSecret-0fe0456-01")
            {
                return View("Error");
            }

            var vendors = db.EcomUsers
                            .Include(m => User)
                            .Where(m => m.PrimaryRole == "Vendor")
                            .ToList();

            var model = new AdminFixActivesViewModel
            {
                Vendors = vendors.Select(m => m.User.UserName).ToList(),
                ProductsDeactivated = new Dictionary<string, List<string>>()
            };

            foreach (var vendor in vendors)
            {
                var activeProds = GetActiveProductsCount(vendor.ApplicationUserId);
                var maxProds = GetMaxProductsAllowed(vendor.ApplicationUserId);

                if (activeProds > maxProds)
                {
                    var prodsToDeActivate = db.Products
                        .Where(m => m.IsActive)
                        .OrderByDescending(m => m.DateAdded)
                        .Take(activeProds - maxProds)
                        .ToList();

                    foreach (var prod in prodsToDeActivate)
                    {
                        prod.IsActive = false;
                        db.Entry(prod).State = EntityState.Modified;
                    }

                    model.ProductsDeactivated.Add(vendor.ApplicationUserId,
                        prodsToDeActivate.Select(m => m.Name).ToList());
                    db.SaveChanges();
                }
            }

            return View(model);
        }

        public ActionResult ResetPassword(int? id)
        {
            if (id == null) return View("Error");
            var ecomUser = db.EcomUsers
                .Include(m => m.User)
                .FirstOrDefault(m => m.Id == id);
            return View(new AdminVendorsResetPasswordViewModel
            {
                VendorId = ecomUser.Id,
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

        private int GetActiveProductsCount(string id)
        {
            var vendorId = id;
            var vendor = db.EcomUsers
                        .Include(m => m.User)
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.ApplicationUserId == vendorId);

            var prods = db.Products
                        .Include(m => m.Category)
                        .Include(m => m.Category.Store)
                        .Where(m => m.Category.Store.EcomUserId
                            == vendor.Id && m.IsActive == true)
                        .ToList();

            return prods.Count;
        }

        private int GetMaxProductsAllowed(string id)
        {
            var vendorId = id;
            var vendor = db.EcomUsers
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.ApplicationUserId == vendorId);

            var pkg = db.VendorPlans
                        .FirstOrDefault(m => m.Id ==
                        vendor.VendorDetails.ActivePlan.VendorPlanId);

            return pkg.MaxProducts;
        }
    }
}