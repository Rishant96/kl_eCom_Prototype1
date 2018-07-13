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
                ActivePackage = db.ActivePackages
                                  .Include(m => m.Package)
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
            var activePckg = db.ActivePackages
                                  .Include(m => m.Package)
                                  .Include(m => m.PaymentDetails)
                                  .FirstOrDefault(m => m.ApplicationUserId == vendor.Id);
            return View(new AdminVendorsPlanChangeViewModel {
                VendorName = vendor.FirstName + " " + vendor.LastName,
                VendorId = vendor.Id,
                CurrentPackage = activePckg.Package.DisplayName,
                NewPlanId = chngRqst.VendorPackageId,
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
                    var oldPkg = db.ActivePackages.FirstOrDefault(m => m.ApplicationUserId == model.VendorId);
                    db.ActivePackages.Remove(oldPkg);

                    changeRequest.Status = Utilities.RequestStatus.Accepted;

                    var vendor = db.Users
                                .Include(m => m.VendorDetails)
                                .FirstOrDefault(m => m.Id == model.VendorId);

                    var pkg = db.VendorPackages.FirstOrDefault(m => m.Id == model.NewPlanId);

                    if (!model.IsPaidFor)
                    {
                        vendor.VendorDetails.ActivePackage = new Utilities.ActivePackage
                        {
                            ApplicationUserId = model.VendorId,
                            IsPaidFor = false,
                            Package = pkg,
                            VendorPackageId = model.NewPlanId,
                            VendorPaymentDetailsId = null
                        };
                    }
                    else
                    {
                        vendor.VendorDetails.ActivePackage = new Utilities.ActivePackage
                        {
                            ApplicationUserId = model.VendorId,
                            IsPaidFor = true,
                            Package = pkg,
                            VendorPackageId = model.NewPlanId,
                            PaymentDetails = new Utilities.VendorPaymentDetails
                            {
                                ApplicationUserId = model.VendorId,
                                VendorPackageId = model.NewPlanId,
                                PaymentMode = model.PaymentMode,
                                KL_Notes = model.Notes
                            }
                        };
                    }
                    
                    db.Entry(vendor).State = EntityState.Modified;
                    db.SaveChanges();

                    var vendor2 = db.Users
                                .Include(m => m.VendorDetails)
                                .Include(m => m.VendorDetails.ActivePackage)
                                .FirstOrDefault(m => m.Id == model.VendorId);
                }
                changeRequest.DecisionDate = DateTime.Now;
                db.Entry(changeRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id  = model.VendorId });
            }
            return View(model);
        }
    }
}