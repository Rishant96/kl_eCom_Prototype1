using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace kl_eCom.Web.Controllers
{
    [Authorize(Roles = "Customer, Vendor")]
    public class CustomerController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOrders()
        {
            var usrId = User.Identity.GetUserId();

            return View(db.Orders
                .Include(m => m.OrderItems)
                .Where(m => m.ApplicationUserId == usrId)
                .OrderByDescending(m => m.OrderDate)
                .ToList());
        }

        public ActionResult Cancellation(int? id)
        {
            if (id == null) return View("Error");

            return View(new CustomerCancellationViewModel {
                Id = (int)id,
                Order = db.Orders
                    .Include(m => m.OrderItems)
                    .FirstOrDefault(m => m.Id == id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancellation(CustomerCancellationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = db.Orders
                    .FirstOrDefault(m => m.Id == model.Id);

                string itemsStr = Request.Form["CanceledItems"];

                if (itemsStr is null) itemsStr = "";
                var cnclsList = itemsStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (cnclsList is null) cnclsList = new string[] { };

                for (int i = 0; i < cnclsList.Count(); i++)
                {
                    if (cnclsList[i] == "") continue;
                    int id = int.Parse(cnclsList[i]);
                    var orderItm = db.OrderItems.FirstOrDefault(m => m.Id == id);
                    orderItm.Status = Utilities.OrderStatus.CancellationRequested;
                    db.Entry(orderItm).State = EntityState.Modified;
                }

                db.SaveChanges();

                return RedirectToAction("MyOrders");
            }
            return View("Error");
        }

        public ActionResult Addresses()
        {
            var usrId = User.Identity.GetUserId();

            return View(new CustomerAddressesIndexViewModel {
                Addresses = db.Addresses
                    .Where(m => m.ApplicationUserId == usrId)
                    .ToList()
            });
        }

        public ActionResult CreateAddress(string returnUrl = "")
        {
            if (returnUrl != "" && Url.IsLocalUrl(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View(new CustomerCreateAddressViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAddress(CustomerCreateAddressViewModel model)
        {
            var usrId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Addresses.Add(new Utilities.Address {
                    ApplicationUserId = usrId,
                    Name = model.Name,
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    Line3 = model.Line3,
                    City = model.City,
                    Place = model.Place,
                    Zip = model.Zip,
                    State = model.State,
                    Country = model.Country
                });
                db.SaveChanges();

                if (TempData["ReturnUrl"] is string returnUrl)
                        return Redirect(returnUrl);

                return RedirectToAction("Addresses");
            }
            return View(model);
        }

        public ActionResult EditAddress(int? id, string returnUrl = "")
        {
            if (returnUrl != "" && Url.IsLocalUrl(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            if (id == null) return View("Error");
            var usrId = User.Identity.GetUserId();
            var addr = db.Addresses.FirstOrDefault(m => m.Id == id 
                        && m.ApplicationUserId == usrId);
            if (addr == null) return View("Error");
            return View(new CustomerEditAddressViewModel {
                Id = (int)id,
                Name = addr.Name,
                Line1 = addr.Line1,
                Line2 = addr.Line2,
                Line3 = addr.Line3,
                City = addr.City,
                State = addr.State,
                Place = addr.Place,
                Zip = addr.Zip,
                Country = addr.Country
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(CustomerEditAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var addr = db.Addresses.FirstOrDefault(m => m.Id == model.Id);
                if (addr != null)
                {
                    addr.Name = model.Name;
                    addr.Line1 = model.Line1;
                    addr.Line2 = model.Line2;
                    addr.Line3 = model.Line3;
                    addr.City = model.City;
                    addr.State = model.State;
                    addr.Zip = model.Zip;
                    addr.Place = model.Place;
                    addr.Country = addr.Country;
                    db.Entry(addr).State = EntityState.Modified;
                    db.SaveChanges();

                    if (TempData["ReturnUrl"] is string returnUrl)
                        return Redirect(returnUrl);

                    return RedirectToAction("Addresses");
                }
                else
                {
                    return View("Error");
                }
            }
            return View(model);
        }

        public ActionResult DeleteAddress(int? id, string returnUrl = "")
        {
            if (returnUrl != "" && Url.IsLocalUrl(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            if (id == null) return View("Error");
            var usrId = User.Identity.GetUserId();
            var addr = db.Addresses.FirstOrDefault(m => m.Id == id
                        && m.ApplicationUserId == usrId);
            if (addr == null) return View("Error");
            return View(new CustomerDeleteAddressViewModel
            {
                Id = (int)id,
                Name = addr.Name,
                Line1 = addr.Line1,
                Line2 = addr.Line2,
                Line3 = addr.Line3,
                City = addr.City,
                State = addr.State,
                Place = addr.Place,
                Zip = addr.Zip,
                Country = addr.Country
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAddress(CustomerDeleteAddressViewModel model)
        {
            if (model.Id != 0)
            {
                var addr = db.Addresses.FirstOrDefault(m => m.Id == model.Id);
                if (addr != null)
                {
                    db.Entry(addr).State = EntityState.Deleted;
                    db.SaveChanges();

                    if (TempData["ReturnUrl"] is string returnUrl)
                        return Redirect(returnUrl);

                    return RedirectToAction("Addresses");
                }
                else
                {
                    return View("Error");
                }
            }
            return View(model);
        }
    }
}