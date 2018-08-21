using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class VendorsController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: KL_Admin/Vendors
        public ActionResult Index()
        {
            return View(new AdminVendorsIndexViewModel {
                Vendors = db.Users
                            .Include(m => m.VendorDetails)
                            .Include(m => m.VendorDetails.ActivePlan)
                            .Where(m => m.PrimaryRole == "Vendor")
                            .ToList()
            });
        }

        public ActionResult Details(string id)
        {
            var vendor = db.Users
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.Id == id);
            if (vendor == null || vendor.VendorDetails == null) return View("Error");

            return View(new AdminVendorsDetailsViewModel {
                Vendor = vendor,
                VendorDetails = vendor.VendorDetails,
                ActivePackage = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetail)
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id),
                DowngradeRequest = db.VendorDowngradeRecords.FirstOrDefault(m => m.ApplicationUserId == vendor.Id)
            });
        }

        public ActionResult EditDomain(string id)
        {
            var vendor = db.Users
                            .Include(m => m.VendorDetails)
                            .FirstOrDefault(m => m.Id == id);
            if (vendor == null) return View("Error");
            return View(new AdminVendorsDomainEditViewModel {
                FullName = vendor.FirstName + " " + vendor.LastName,
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
                var vendor = db.Users
                            .Include(m => m.VendorDetails)
                            .Include(m => m.VendorDetails.ActivePlan)
                            .Include(m => m.VendorDetails.ActivePlan.Plan)
                            .FirstOrDefault(m => m.Id == model.Id);
                vendor.VendorDetails.DomainRegistrationDate = model.DomainDate;

                var date = (DateTime)model.DomainDate;
                var daysNotToCharge = (vendor.VendorDetails.ActivePlan.EndDate -
                                          date.AddYears(DateTime.Now.Year - date.Year + 1))
                                          .Days;
                var validRefund = ((vendor.VendorDetails.ActivePlan.Plan.Price
                                    * (1 + (vendor.VendorDetails.ActivePlan.Plan.GST / 100)))
                                    / 365) * daysNotToCharge;

                db.VendorPlanChangeRecord.Add(new VendorPlanChangeRecord {
                    ApplicationUserId = vendor.Id,
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
            var vendor = db.Users
                        .Include(m => m.VendorDetails)
                        .FirstOrDefault(m => m.Id == id);
            if (vendor == null || vendor.VendorDetails == null) return View("Error");
            var activePckg = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetail)
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id);
            return View(new AdminVendorsPlanChangeViewModel {
                VendorName = vendor.FirstName + " " + vendor.LastName,
                VendorId = vendor.Id,
                ActivePlan = activePckg,
                Amount = activePckg.Balance ?? 0.0f,
                DowngradeRecord = db.VendorDowngradeRecords
                    .Include(m => m.NewPlan)
                    .FirstOrDefault(m => m.ApplicationUserId == vendor.Id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlanChange(AdminVendorsPlanChangeViewModel model, bool flag)
        {
            if (flag)
            {
                var activePkg = db.ActivePlans
                        .Include(m => m.Plan)
                        .FirstOrDefault(m => m.ApplicationUserId == model.VendorId);

                var downgradeRecord = db.VendorDowngradeRecords
                                .Include(m => m.NewPlan)
                                .FirstOrDefault(m => m.ApplicationUserId == model.VendorId);

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

            var mode = int.Parse(Request.Form["Mode"]);
            if (!string.IsNullOrEmpty(model.VendorId))
            {
                var activePkg = db.ActivePlans
                        .FirstOrDefault(m => m.ApplicationUserId == model.VendorId);
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

            var vendors = db.Users
                            .Where(m => m.PrimaryRole == "Vendor")
                            .ToList();

            var model = new AdminFixActivesViewModel
            {
                Vendors = vendors.Select(m => m.UserName).ToList(),
                ProductsDeactivated = new Dictionary<string, List<string>>()
            };

            foreach (var vendor in vendors)
            {
                var activeProds = GetActiveProductsCount(vendor.Id);
                var maxProds = GetMaxProductsAllowed(vendor.Id);

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

                    model.ProductsDeactivated.Add(vendor.Id,
                        prodsToDeActivate.Select(m => m.Name).ToList());
                    db.SaveChanges();
                }
            }

            return View(model);
        }

        private int GetActiveProductsCount(string id)
        {
            var vendorId = id;
            var vendor = db.Users
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.Id == vendorId);

            var prods = db.Products
                        .Include(m => m.Category)
                        .Include(m => m.Category.Store)
                        .Where(m => m.Category.Store.ApplicationUserId == vendor.Id && m.IsActive == true)
                        .ToList();

            return prods.Count;
        }

        private int GetMaxProductsAllowed(string id)
        {
            var vendorId = id;
            var vendor = db.Users
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.Id == vendorId);

            var pkg = db.VendorPlans
                        .FirstOrDefault(m => m.Id ==
                        vendor.VendorDetails.ActivePlan.VendorPlanId);

            return pkg.MaxProducts;
        }
    }
}