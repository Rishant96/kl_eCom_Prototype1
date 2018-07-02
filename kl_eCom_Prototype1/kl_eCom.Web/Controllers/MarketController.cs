using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace kl_eCom.Web.Controllers
{
    public class MarketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Market
        public ActionResult Index()
        {
            var vendors = (db.Roles.FirstOrDefault(m => m.Name == "Vendor")).Users;
            var model = new MarketIndexViewModel { Vendors = new Dictionary<string, string>() };
            foreach (var vendor in vendors)
            {
                var v = db.Users
                    .Include(m => m.Stores)
                    .Include(m => m.VendorDetails)
                    .FirstOrDefault(m => m.Id == vendor.UserId);
                if(v.Stores.Count > 0) 
                    model.Vendors.Add(v.Id, 
                        v.VendorDetails.BusinessName);
            }
            return View(model);
        }

        public ActionResult Shops(string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId))
                return View("Index");
            var shops = db.Stores.Where(m => m.ApplicationUserId == vendorId).ToList();
            if (shops.Count == 1) return RedirectToAction("Index", "Shop", new { id = shops.First().Id });
            return View(new MarketShopsViewModel {
                Shops = shops    
            });
        }
    }
}