using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Utilities;
using kl_eCom.Web.Entities;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class StoreController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Store
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            ViewBag.VendorId = id;
            var stores = db.Stores.Where(m => m.ApplicationUserId == id).ToList();
            var model = new StoreIndexViewModel { Stores = stores,
                Stocks = new Dictionary<Entities.Store, List<Entities.Stock>>() };
            foreach (var store in stores)
            {
                model.Stocks.Add(store,
                    db.Stocks
                    .Where(m => m.StoreId == store.Id)
                    .Include(m => m.Product)
                    .Include(m => m.Product.Category)
                    .ToList()
                );
            }
            return View(model);
        }

        public ActionResult Create(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            TempData["vendorId"] = id;
            return View(new StoreCreateViewModel() { });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StoreCreateViewModel model)
        {
            string vendorId = TempData["vendorId"] as string;
            if(ModelState.IsValid)
            {
                var storeAddr = new StoreAddress {
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    Line3 = model.Line3,
                    Zip = model.Zip,
                    State = model.State,
                    Country = model.Country
                };
                var store = new Store {
                    Name = model.Name,
                    Address = storeAddr,
                    ApplicationUserId = vendorId
                };
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = vendorId });
            }
            TempData["vendorId"] = vendorId;
            return View(model);
        }
    }
}