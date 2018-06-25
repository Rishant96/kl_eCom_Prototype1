using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                model.Vendors.Add(vendor.UserId, 
                    (db.Users.FirstOrDefault(m => m.Id == vendor.UserId).UserName));
            }
            return View(model);
        }

        public ActionResult Shops(string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId))
                return View("Index");
            return View(new MarketShopsViewModel {
                Shops = db.Stores.Where(m => m.ApplicationUserId == vendorId).ToList()
            });
        }
    }
}