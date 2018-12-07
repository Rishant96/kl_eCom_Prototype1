using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;

namespace kl_eCom.Web.Controllers
{
    public class MarketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Market
        public ActionResult Index()
        {
            var vendors = (db.Roles.OrderBy(m => m.Name)
                .FirstOrDefault(m => m.Name == "Vendor")).Users;
            var model = new MarketIndexViewModel { Vendors = new Dictionary<string, string>() };
            foreach (var vendor in vendors)
            {
                if (vendor is null) continue;
                var v = db.EcomUsers
                    .Include(m => m.Stores)
                    .Include(m => m.VendorDetails)
                    .FirstOrDefault(m => m.ApplicationUserId == vendor.UserId);

                if (v != null && v.Stores != null && v.Stores.Count > 0) 
                    model.Vendors.Add(v.ApplicationUserId, 
                        v.VendorDetails.BusinessName);
            }
            return View(model);
        }

        public ActionResult Shops(string vendorId, bool extLink = false)
        {
            if (extLink)
            {
                // Begin Lockout 
                Session["extVendor"] = vendorId;
                if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
                {
                    // Associate with Vendor
                }
            }
            if (string.IsNullOrEmpty(vendorId))
                return View("Index");
            var user = db.EcomUsers
                .FirstOrDefault(m => m.ApplicationUserId == vendorId);
            if (user is null) return View("Error");
            var shops = db.Stores.Where(m => m.EcomUserId == user.Id).ToList();
            if (shops.Count == 1) return RedirectToAction("Index", "Shop", new { id = shops.First().Id });
            return View(new MarketShopsViewModel {
                Shops = shops    
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search()
        {
            var searchQuery = Request.Form["SearchQuery"];
            
            return RedirectToAction("Products", "Shop", new {
                searchQuery = searchQuery.Trim()
            });
        }
        
        [AllowAnonymous]
        public ActionResult Autocomplete(string prefix)
        {
            var prodList = db.Products
                             .Where(m => m.Name.ToUpper().Contains(prefix.ToUpper()))
                             .OrderBy(m => m.Name)
                             .Select(m => new { m.Name, ID = m.Name })
                             .Distinct()
                             .ToList();

            var catList = db.Categories
                             .Where(m => m.Name.ToUpper().Contains(prefix.ToUpper()))
                             .OrderBy(m => m.Name)
                             .Select(m => new { m.Name, ID = m.Name })
                             .Distinct()
                             .ToList();

            var result = catList.Concat(prodList).ToList();
            return Json( result, JsonRequestBehavior.AllowGet);
        }
    }
}