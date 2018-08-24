﻿using kl_eCom.Web.Areas.Vendors.Models;
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
            var stores = db.Stores.Where(m => m.ApplicationUserId == id).ToList();
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

        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            //if (db.Stores.FirstOrDefault(m => m.ApplicationUserId == id) != null) return View("MultipleStores");
            var vendor = db.Users
                        .Include(m => m.VendorDetails)
                        .FirstOrDefault(m => m.Id == id);
            TempData["vendorId"] = id;
            return View(new StoreCreateViewModel() {
                State = vendor.VendorDetails.State,
                Zip = vendor.VendorDetails.Zip,
                Country = "India",
                Name = vendor.VendorDetails.BusinessName,
                CurrencyType = "₹"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StoreCreateViewModel model)
        {   
            string vendorId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(vendorId)) return View("Error");
            if (ModelState.IsValid)
            {
                var storeAddr = new StoreAddress
                {
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    Line3 = model.Line3,
                    Zip = model.Zip,
                    State = model.State,
                    Country = model.Country
                };
                var store = new Store
                {
                    Name = model.Name,
                    Address = storeAddr,
                    ApplicationUserId = vendorId,
                    DefaultCurrencyType = model.CurrencyType
                };
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            var store = db.Stores
                .Include(m => m.Vendor)
                .Include(m => m.Categories)
                .Include(m => m.Address)
                .FirstOrDefault(m => m.Id == id);

            if (store is null) return RedirectToAction("Index");

            return View(new StoreDetailsViewModel
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Vendor = store.Vendor,
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

            return View(new StoreEditViewModel {
                Id = store.Id,
                Name = store.Name,
                AddrName = store.Address.Name,
                CurrencyType = store.DefaultCurrencyType,
                Line1 = store.Address.Line1,
                Line2 = store.Address.Line2,
                Line3 = store.Address.Line3,
                Zip = store.Address.Zip,
                State = store.Address.State,
                Country = store.Address.Country
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
                store.Address.State = model.State;
                store.Address.Country = model.Country;
                store.DefaultCurrencyType = model.CurrencyType;

                db.Entry(store).State = EntityState.Modified;
                db.Entry(store.Address).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
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