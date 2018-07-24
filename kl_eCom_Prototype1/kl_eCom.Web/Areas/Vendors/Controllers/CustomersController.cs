using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Entities;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class CustomersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Customers
        public ActionResult Index()
        {
            var vndrId = User.Identity.GetUserId();

            var customers = db.Refferals
                .Where(m => m.VendorId == vndrId)
                .Select(m => m.CustomerId)
                .ToList();

            var model = new CustomersIndexViewModel
            {
                Customers = db.Users
                    .Where(m => customers.Contains(m.Id))
                    .OrderBy(m => m.FirstName + " " + m.LastName)
                    .ToList(),
                Buyers = new Dictionary<string, bool>(),
                Registrations = new Dictionary<string, bool>()
            };

            foreach (var customer in model.Customers)
            {
                var rel = db.Refferals
                    .FirstOrDefault(m => m.CustomerId == customer.Id 
                    && m.VendorId == vndrId);

                if (rel.IsBuyer == true)
                    model.Buyers.Add(customer.Id, true);
                else
                    model.Buyers.Add(customer.Id, false);

                if (rel.IsRegisteredUser == true)
                    model.Registrations.Add(customer.Id, true);
                else
                    model.Registrations.Add(customer.Id, false);
            }

            return View(model);
        }

        public ActionResult Orders(string id)
        {


            return View();
        }
    }
}