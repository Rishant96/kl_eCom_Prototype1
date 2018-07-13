using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class PlansController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: KL_Admin/Plans/Create
        public ActionResult Create()
        {
            return View(new AdminPlansCreateViewModel() { ValidityPeriod = 1, MaxProducts = 10 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminPlansCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.VendorPackages.Add(new Utilities.VendorPackage {
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    ValidityPeriod = (model.IsExpirable) ? model.ValidityPeriod : null,
                    Price = model.Price,
                    IsActive = model.IsActive,
                    MaxProducts = model.MaxProducts,
                    IsEnabled = model.IsEnabled
                });
                db.SaveChanges();
                return RedirectToAction("Plans", controllerName: "Admin");
            }
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return View("Error");
            var model = db.VendorPackages.FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(new AdminPlansEditViewModel {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName,
                IsActive = model.IsActive,
                ValidityPeriod = model.ValidityPeriod,
                Price = model.Price,
                MaxProducts = model.MaxProducts,
                IsExpirable = (model.ValidityPeriod != null) ? true : false,
                IsEnabled = model.IsEnabled
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminPlansEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var plan = db.VendorPackages.FirstOrDefault(m => m.Id == model.Id);
                plan.Name = model.Name;
                plan.DisplayName = model.DisplayName;
                plan.ValidityPeriod = (model.IsExpirable) ? model.ValidityPeriod : null;
                plan.IsActive = model.IsActive;
                plan.Price = model.Price;
                plan.IsEnabled = model.IsEnabled;
                db.Entry(plan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return View("Error");
            var model = db.VendorPackages.FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(VendorPackage model)
        {
            var plan = db.VendorPackages.FirstOrDefault(m => m.Id == model.Id);
            if (plan != null)
            {
                var entry = db.Entry(plan);
                if (entry.State == EntityState.Detached)
                    db.VendorPackages.Attach(plan);
                db.VendorPackages.Remove(plan);
                db.SaveChanges();
                return RedirectToAction("Plans", controllerName: "Admin");
            }
            return View("Error");
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return View("Error");
            var model = db.VendorPackages.FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(new AdminPlansDetailsViewModel { VendorPackage = model });
        }
    }
}