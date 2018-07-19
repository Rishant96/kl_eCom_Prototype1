using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;

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
                ChangeRequest = db.PlanChangeRequests
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id 
                                  && m.Status == Utilities.RequestStatus.Pending),
                ActivePackage = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetails)
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id)
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
                DomainDate = (DateTime) vendor.VendorDetails.DomainRegistrationDate
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
                            .FirstOrDefault(m => m.Id == model.Id);
                vendor.VendorDetails.DomainRegistrationDate = model.DomainDate;
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
            var chngRqst = db.PlanChangeRequests
                             .Include(m => m.RequestedPackage)
                             .FirstOrDefault(m => m.ApplicationUserId == vendor.Id 
                                    && m.Status == Utilities.RequestStatus.Pending);
            var activePckg = db.ActivePlans
                                  .Include(m => m.Plan)
                                  .Include(m => m.PaymentDetails)
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id);
            return View(new AdminVendorsPlanChangeViewModel {
                VendorName = vendor.FirstName + " " + vendor.LastName,
                VendorId = vendor.Id,
                CurrentPackage = activePckg.Plan.DisplayName,
                NewPlanId = chngRqst.VendorPlanId,
                NewPlanName = chngRqst.RequestedPackage.DisplayName,
                NewPlanMaxProds = chngRqst.RequestedPackage.MaxProducts
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlanChange(AdminVendorsPlanChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var changeRequest = db.PlanChangeRequests
                                      .Include(m => m.RequestedPackage)
                                      .FirstOrDefault(m => m.ApplicationUserId == model.VendorId
                                        && m.Status == Utilities.RequestStatus.Pending);
                if (!model.IsAccepted)
                    changeRequest.Status = Utilities.RequestStatus.Dismissed;
                else
                {
                    var oldPkg = db.ActivePlans.FirstOrDefault(m => m.ApplicationUserId == model.VendorId);
                    db.ActivePlans.Remove(oldPkg);

                    changeRequest.Status = Utilities.RequestStatus.Accepted;

                    var vendor = db.Users
                                .Include(m => m.VendorDetails)
                                .FirstOrDefault(m => m.Id == model.VendorId);

                    var pkg = db.VendorPlans.FirstOrDefault(m => m.Id == model.NewPlanId);

                    if (!model.IsPaidFor)
                    {
                        vendor.VendorDetails.ActivePlan = new Utilities.ActivePlan
                        {
                            ApplicationUserId = model.VendorId,
                            IsPaidFor = false,
                            Plan = pkg,
                            VendorPlanId = model.NewPlanId,
                            VendorPaymentDetailsId = null
                        };
                    }
                    else
                    {
                        vendor.VendorDetails.ActivePlan = new Utilities.ActivePlan
                        {
                            ApplicationUserId = model.VendorId,
                            IsPaidFor = true,
                            Plan = pkg,
                            VendorPlanId = model.NewPlanId,
                            PaymentDetails = new Utilities.VendorPaymentDetails
                            {
                                ApplicationUserId = model.VendorId,
                                VendorPlanId = model.NewPlanId,
                                PaymentMode = model.PaymentMode,
                                Details = model.Notes
                            }
                        };
                    }
                    
                    db.Entry(vendor).State = EntityState.Modified;
                    db.SaveChanges();

                    var vendor2 = db.Users
                                .Include(m => m.VendorDetails)
                                .Include(m => m.VendorDetails.ActivePlan)
                                .FirstOrDefault(m => m.Id == model.VendorId);
                }
                changeRequest.DecisionDate = DateTime.Now;
                db.Entry(changeRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id  = model.VendorId });
            }
            return View(model);
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