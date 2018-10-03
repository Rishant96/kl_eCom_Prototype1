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
using Microsoft.AspNet.Identity;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class StoreController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Store
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == id);
            if (user is null) return View("Error");
            var stores = db.Stores.Where(m => m.EcomUserId == user.Id).ToList();
            var model = new StoreIndexViewModel
            {
                Stores = stores,
                Stocks = new Dictionary<Entities.Store, List<Entities.Stock>>()
            };
            foreach (var store in stores)
            {
                model.Stocks.Add(store,
                    db.Stocks
                    .Where(m => m.StoreId == store.Id)
                    .Include(m => m.Product)
                    .Include(m => m.Product.Category)
                    .Include(m => m.Store)
                    .ToList()
                );
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult GetStates(string idStr = "")
        {
            if (string.IsNullOrEmpty(idStr)) return null;
            int id = int.Parse(idStr);

            if (id == 0) return null;

            IEnumerable<SelectListItem> states = db.States
                .Where(m => m.CountryId == id)
                .OrderBy(m => m.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCities(string idStr = "")
        {
            if (string.IsNullOrEmpty(idStr)) return null;
            int id = int.Parse(idStr);

            if (id == 0) return null;

            IEnumerable<SelectListItem> cities = db.Places
                .Where(m => m.StateId == id)
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMarkets(string idStr = "")
        {
            if (string.IsNullOrEmpty(idStr)) return null;
            int id = int.Parse(idStr);

            if (id == 0) return null;

            IEnumerable<SelectListItem> markets = db.Markets
                .Where(m => m.PlaceId == id)
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return Json(markets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            //if (db.Stores.FirstOrDefault(m => m.ApplicationUserId == id) != null) return View("MultipleStores");
            var vendor = db.EcomUsers
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.BusinessAddress)
                        .FirstOrDefault(m => m.ApplicationUserId == id);
            TempData["vendorId"] = id;

            List<SelectListItem> countries = db.Countries
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            List<SelectListItem> states = (vendor.VendorDetails.BusinessAddress.CountryId == 0) ?
                new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } : 
                db.States.OrderBy(m => m.Name)
                    .Where(m => m.CountryId == vendor.VendorDetails
                        .BusinessAddress.CountryId)
                    .Select(s => new SelectListItem {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();

            List<SelectListItem> cities = (vendor.VendorDetails.BusinessAddress.StateId == 0) ?
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } :
                db.Places.OrderBy(m => m.Name)
                    .Where(m => m.StateId == vendor.VendorDetails
                        .BusinessAddress.StateId)
                    .Select(p => new SelectListItem {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    }).ToList();

            List<SelectListItem> markets = (vendor.VendorDetails.BusinessAddress.PlaceId is null) ?
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } : 
                db.Markets.OrderBy(m => m.Name)
                    .Where(m => m.PlaceId == vendor.VendorDetails
                        .BusinessAddress.PlaceId)
                    .Select(m => new SelectListItem {
                        Value = m.Id.ToString(),
                        Text = m.Name
                    }).ToList();

            return View(new StoreCreateViewModel() {
                State = vendor.VendorDetails.BusinessAddress.StateId,
                Zip = vendor.VendorDetails.BusinessAddress.Zip,
                Country = vendor.VendorDetails.BusinessAddress.CountryId,
                City = vendor.VendorDetails.BusinessAddress.PlaceId ?? 0,
                Market = vendor.VendorDetails.BusinessAddress.MarketId,
                Name = vendor.VendorDetails.BusinessName,
                CurrencyType = "₹",
                Countries = countries,
                States = states,
                Cities = cities,
                Markets = markets
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StoreCreateViewModel model)
        {   
            string vendorId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(vendorId)) return View("Error");
            var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == vendorId);
            if (user is null) return View("Error");
            if (ModelState.IsValid)
            {
                var storeAddr = new StoreAddress
                {
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    Line3 = model.Line3,
                    Zip = model.Zip,
                    StateId = model.State,
                    CountryId = model.Country,
                    PlaceId = model.City,
                    MarketId = model.Market
                };

                var store = new Store
                {
                    Name = model.Name,
                    Address = storeAddr,
                    EcomUserId = user.Id,
                    DefaultCurrencyType = model.CurrencyType
                };

                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }

        public ActionResult Details(int? id)
        {
            var store = db.Stores
                .Include(m => m.Vendor)
                .Include(m => m.Vendor.User)
                .Include(m => m.Categories)
                .Include(m => m.Address)
                .Include(m => m.Address.Country)
                .Include(m => m.Address.State)
                .Include(m => m.Address.Place)
                .Include(m => m.Address.Market)
                .FirstOrDefault(m => m.Id == id);

            if (store is null) return RedirectToAction("Index");

            return View(new StoreDetailsViewModel
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Vendor = store.Vendor.User,
                Categories = store.Categories,
                CurrencyType = store.DefaultCurrencyType
            });
        }

        public ActionResult Edit(int? id)
        {
            var store = db.Stores
                .Include(m => m.Address)
                .FirstOrDefault(m => m.Id == id);

            if (store is null) return RedirectToAction("Index");

            List<SelectListItem> countries = db.Countries
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            
            List<SelectListItem> states = (store.Address.CountryId == 0) ?
                new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } :
                db.States.OrderBy(m => m.Name)
                    .Where(m => m.CountryId == store.Address.CountryId)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();

            List<SelectListItem> cities = (store.Address.StateId == 0) ?
                new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } :
                db.Places.OrderBy(m => m.Name)
                    .Where(m => m.StateId == store.Address.StateId)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();

            List<SelectListItem> markets = (store.Address.PlaceId == 0) ?
                new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = null,
                        Text = ""
                    }
                } :
                db.Markets.OrderBy(m => m.Name)
                    .Where(m => m.PlaceId == store.Address.PlaceId)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();

            return View(new StoreEditViewModel {
                Id = store.Id,
                Name = store.Name,
                AddrName = store.Address.Name,
                CurrencyType = store.DefaultCurrencyType,
                Line1 = store.Address.Line1,
                Line2 = store.Address.Line2,
                Line3 = store.Address.Line3,
                Zip = store.Address.Zip,
                State = store.Address.StateId,
                Country = store.Address.CountryId,
                City = store.Address.PlaceId ?? 0,
                Market = store.Address.MarketId ?? 0,
                Landmark = store.Address.Landmark,
                Countries = countries,
                States = states,
                Cities = cities,
                Markets = markets
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StoreEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var store = db.Stores
                    .Include(m => m.Address)
                    .FirstOrDefault(m => m.Id == model.Id);

                if (store == null) return View("Error");
                store.Name = model.Name;
                store.Address.Name = model.AddrName;
                store.Address.Line1 = model.Line1;
                store.Address.Line2 = model.Line2;
                store.Address.Line3 = model.Line3;
                store.Address.Zip = model.Zip;
                store.Address.StateId = model.State;
                store.Address.CountryId = model.Country;
                store.Address.PlaceId = model.City;
                store.Address.MarketId = model.Market;
                store.Address.Landmark = model.Landmark;
                store.DefaultCurrencyType = model.CurrencyType;

                db.Entry(store).State = EntityState.Modified;
                db.Entry(store.Address).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = model.Id });
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null) return View("Error");
            var store = db.Stores.FirstOrDefault(m => m.Id == id);
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Store store)
        {
            if (store == null) return View("Error");
            if (store.Id == 0) return View("Error");
            var entry = db.Entry(store);
            if (entry.State == EntityState.Detached)
            {
                db.Stores.Attach(store);
            }
            try
            {
                db.Stores.Remove(store);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteStock(int? id)
        {
            if (id == null) return View("Error");
            var model = db.Stocks
                .Include(m => m.Product)
                .Include(m => m.Store)
                .FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStock(Stock model)
        {
            var entry = db.Entry(model);
            if (entry.State == EntityState.Detached)
                db.Stocks.Attach(model);
            db.Stocks.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    } 
}