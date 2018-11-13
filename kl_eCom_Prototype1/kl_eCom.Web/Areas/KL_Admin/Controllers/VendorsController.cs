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
using kl_eCom.Web.Entities;
using System.Threading.Tasks;

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

        public ActionResult Delete(int? id)
        {
            if (id is null || id == 0) return View("Error");
            var model = db.EcomUsers
                            .Include(m => m.User)
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.Id == id);
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
                            .Include(m => m.VendorDetails.ActivePlan)
                            .FirstOrDefault(m => m.ApplicationUserId == model.ApplicationUserId);

            var stocks = db.Stocks
                .Include(m => m.Store)
                .Include(m => m.Store.Categories)
                .Include(m => m.Product)
                .Include(m => m.Product.Category)
                .Include(m => m.Product.Category.Parent)
                .Where(m => m.Store.EcomUserId == vendor.Id);

            foreach (var stock in stocks)
            {
                db.Stocks.Attach(stock);
                db.Products.Attach(stock.Product);
                
                try
                {
                    db.Categories.Attach(stock.Product.Category);
                }
                catch(Exception ex) { }

                try
                {
                    db.Categories.Attach(stock.Product.Category.Parent);
                }
                catch(Exception ex) { }

                db.Stores.Attach(stock.Store);

                db.Stocks.Remove(stock);
                db.Categories.Remove(stock.Product.Category);
                db.Stores.Remove(stock.Store);

                db.SaveChanges();
            }            
            
            db.EcomUsers.Attach(vendor);
            db.VendorDetails.Attach(vendor.VendorDetails);
            db.ActivePlans.Attach(vendor.VendorDetails.ActivePlan);
            db.Users.Attach(vendor.User);

            db.EcomUsers.Remove(vendor);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult EditVendorDetails(int? id)
        {
            if (id is null) return View("Error");

            var vendor = db.EcomUsers
                .Include(m => m.User)
                .Include(m => m.VendorDetails)
                .FirstOrDefault(m => m.Id == id);
            if (vendor is null) return View("Error");

            var model = new AdminVendorsEditDetailsViewModel {
                Id = vendor.Id,
                AppUserId = vendor.ApplicationUserId,
                BusinessName = vendor.VendorDetails.BusinessName,
                FirstName = vendor.User.FirstName,
                LastName = vendor.User.LastName,
                Email = vendor.User.Email
            };

            return View(model);
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
                
                var balanceType = int.Parse(Request.Form["BalanceType"]);
                switch (balanceType)
                {
                    case 1:
                        {
                            activePkg.PaymentDetail = new VendorPlanPaymentDetail
                            {
                                AmountPaid = (float)activePkg.Balance,
                                PaymentDate = DateTime.Now,
                                PaymentType = PaymentType.Other,
                                Notes = "Balance cleared by Admin."
                            };

                            activePkg.Balance = 0.0f;
                            activePkg.PaymentStatus = true;

                            break;
                        }
                    case 2:
                        {
                            var balance = ((activePkg.Plan.Price - downgradeRecord.NewPlan.Price) / 365)
                                            * (activePkg.EndDate - DateTime.Now).Days;


                            activePkg.PaymentDetail = new VendorPlanPaymentDetail
                            {
                                AmountPaid = (float)balance,
                                PaymentDate = DateTime.Now,
                                PaymentType = PaymentType.Other,
                                Notes = "Account credited by Admin."
                            };

                            activePkg.Balance -= balance;
                            activePkg.Balance = (float)Math.Floor((float)activePkg.Balance);

                            if (activePkg.Balance == 0.0f) activePkg.PaymentStatus = true;
                            else if (activePkg.Balance < 0.0f) return View("Error");

                            break;
                        }
                    default:
                        {
                            return View("Error");
                        }
                }
                
                db.Entry(activePkg).State = EntityState.Modified;
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
        public ActionResult ResetPassword(AdminVendorsResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vendorId = db.EcomUsers.FirstOrDefault(m => m.Id == model.VendorId).ApplicationUserId;
                var code = UserManager.GeneratePasswordResetToken(vendorId);
                var result = UserManager.ResetPassword(vendorId, code, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    if (user != null)
                    {
                        SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return View("Error");
        }

        public ActionResult Specializations()
        {
            var allSpecializations = db.Specializations.ToList();

            var baseSpecializations = allSpecializations.Where(m => m.SpecializationId == null).ToList();
            var specializationDict = new Dictionary<Specialization, List<Specialization>>();

            foreach (var specialization in allSpecializations)
            {
                var specList = db.Specializations
                    .Where(m => m.SpecializationId == specialization.Id).ToList();
                specializationDict.Add(specialization, specList);
            }

            return View(new AdminVendorsSpecializationsIndexViewModel {
                BaseSpecializations = baseSpecializations,
                ChildSpecializations = specializationDict
            });
        }

        public ActionResult CreateSpecialization()
        {
            var specializations = db.Specializations
                .Where(m => m.SpecializationId == null)
                .ToList();
            var specializationDict = new Dictionary<string, int>();
            foreach (var specialization in specializations)
            {
                specializationDict.Add(specialization.Name, specialization.Id);
            }
            return View(new AdminVendorsSpecializationCreateViewModel {
                Specializations = specializationDict,
                IsVisible = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSpecialization(AdminVendorsSpecializationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Specializations.Add(new Specialization
                {
                    Name = model.Name,
                    SpecializationId = model.SelectedSpecialization,
                    IsVisible = model.IsVisible
                });
                db.SaveChanges();
                return RedirectToAction("Specializations");
            }

            return View(model);
        }

        public ActionResult SpecializationDetails(int? id)
        {
            if (id == null) return View("Error");
            var specialization = db.Specializations
                    .Include(m => m.ParentSpecialization)
                    .FirstOrDefault(m => m.Id == id);
            if (specialization is null) return View("Error");
            return View(specialization);
        }

        public ActionResult SpecializationEdit(int? id)
        {
            if (id == null) return View("Error");

            var specialization = db.Specializations
                    .FirstOrDefault(m => m.Id == id);
            if (specialization is null) return View("Error");

            var specializations = db.Specializations
                .Where(m => m.SpecializationId == null
                    && m.Id != id)
                .ToList();
            var specializationDict = new Dictionary<string, int>();

            foreach (var spec in specializations)
            {
                specializationDict.Add(spec.Name, spec.Id);
            }
            if (specialization is null) return View("Error");

            return View(new AdminVendorsSpecializationEditViewModel {
                Id = specialization.Id,
                Name = specialization.Name,
                IsVisible = specialization.IsVisible,
                SelectedSpecialization = specialization.SpecializationId,
                Specializations = specializationDict
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SpecializationEdit(AdminVendorsSpecializationEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var specialization = db.Specializations.FirstOrDefault(m => m.Id == model.Id);
                specialization.Name = model.Name;
                specialization.IsVisible = model.IsVisible;
                specialization.SpecializationId = model.SelectedSpecialization;
                db.Entry(specialization).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("SpecializationDetails", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult SpecializationDelete(int? id)
        {
            if (id is null) return View("Error");

            var speciality = db.Specializations.FirstOrDefault(m => m.Id == id);
            if (speciality is null) return View("Error");

            return View(speciality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SpecializationDelete(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                var spec = db.Specializations.FirstOrDefault(m => m.Id == specialization.Id);
                db.Entry(spec).State = EntityState.Deleted;
                var parent = spec.SpecializationId;
                foreach (var childSpec in db.Specializations
                    .Where(m => m.SpecializationId == spec.Id).ToList())
                {
                    childSpec.SpecializationId = parent;
                    db.Entry(childSpec).State = EntityState.Modified;
                }
                db.SaveChanges();
                
                return RedirectToAction("Specializations");
            }

            return View(specialization);
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