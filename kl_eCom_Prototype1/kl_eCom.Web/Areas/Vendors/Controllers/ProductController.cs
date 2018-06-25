using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Utilities;
using kl_eCom.Web.Entities;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private List<string> getAllAttributes(int id)
        {
            var parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == id);
            var attrs = new List<string>();
            while (parent != null)
            {
                foreach (var atr in parent.Attributes.Reverse())
                {
                    attrs.Add(atr.Name);
                }
                parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == parent.CategoryId);
            }
            attrs.Reverse();
            return attrs;
        }

        // GET: Vendors/Product
        public ActionResult Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", controllerName: "Home");
            var prod = db.Products
                .Where(m => m.CategoryId == (int)id)
                .Include(m => m.Specifications)
                .ToList();
            if (prod == null) return RedirectToAction("Index", controllerName: "Home"); 

            var model = new ProductIndexViewModel { Products = prod, Attributes = getAllAttributes((int)id) };
            var atrs = model.Attributes;

            for (int j=0; j < model.Products.Count; j++)
            { 
                for (int i = 0; i < atrs.Count; i++)
                {
                    if (model.Products.ElementAt(j).SpecificationsDict == null)
                        model.Products.ElementAt(j).SpecificationsDict = new Dictionary<string, Specification>();
                    model.Products.ElementAt(j).SpecificationsDict.Add(
                        atrs.ElementAt(i),
                        model.Products.ElementAt(j).Specifications.ElementAt(i)
                    );
                }
            }
            ViewBag.catId = id;
            ViewBag.storeId = (db.Categories.FirstOrDefault(m => m.Id == id)).StoreId;
            return View(model);
        }

        public ActionResult Create(int? catId)
        {
            if (catId == null) return RedirectToAction("Index", controllerName: "Home");
            TempData["catId"] = catId;
            var parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == catId);
            var model = new ProductCreateViewModel { Specifications = new Dictionary<string, string>() };
            model.Attributes = new List<string>();
            while (parent != null)
            {
                foreach(var atr in parent.Attributes.Reverse())
                {
                    model.Attributes.Add(atr.Name);
                    model.Specifications.Add(atr.Name, "");
                }
                parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == parent.CategoryId);
            }
            model.Attributes.Reverse();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCreateViewModel model)
        {
            int? catId = TempData["catId"] as int?;
            if (catId == null) return RedirectToAction("Index", controllerName: "Home");

            if (ModelState.IsValid)
            {
                var prod = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Manufacturer = model.Manufacturer,
                    CategoryId = (int)catId,
                    Specifications = new List<Specification>()
                };
                foreach(var spec in model.Specifications.Values)
                {
                    prod.Specifications.Add(new Specification { Value = spec });
                }

                db.Products.Add(prod);
                db.SaveChanges();

                return RedirectToAction("Index", new { id = catId });
            }

            TempData["catId"] = catId;
            model.Attributes = new List<string>();
            foreach(var spec in model.Specifications.Keys)
            {
                model.Attributes.Add(spec);
            }
            return View(model);
        }

        public ActionResult Stock(int? prodId, int? storeId)
        {
            if (prodId == null || storeId == null) return RedirectToAction("Index", controllerName: "Home");
            TempData["prodId"] = prodId;
            TempData["storeId"] = storeId;
            var oldStock = db.Stocks.FirstOrDefault(m => m.ProductId == prodId && m.StoreId == storeId);
            var model = new ProductStockViewModel { Product = db.Products.FirstOrDefault(m => m.Id == prodId) };
            if (oldStock != null)
            {
                model.Price = oldStock.Price;
                model.Stock = oldStock.CurrentStock;
            }
            if(model.Product == null) return RedirectToAction("Index", controllerName: "Home");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stock(ProductStockViewModel model)
        {
            int? prodId = TempData["prodId"] as int?;
            int? storeId = TempData["storeId"] as int?;
            if (prodId == null || storeId == null) return RedirectToAction("Index", controllerName: "Home");
            var oldStock = db.Stocks.FirstOrDefault(m => m.ProductId == prodId && m.StoreId == storeId);

            if (ModelState.IsValid)
            {
                if (oldStock is null)
                {
                    db.Stocks.Add(
                        new Stock
                        {
                            CurrentStock = model.Stock,
                            Price = model.Price,
                            ProductId = (int)prodId,
                            StoreId = (int)storeId,
                        }
                    );
                }
                else
                {
                    oldStock.CurrentStock = model.Stock;
                    oldStock.Price = model.Price;
                    db.Entry(oldStock).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index", controllerName: "Home");
            }

            TempData["prodId"] = prodId;
            TempData["storeId"] = storeId;
            return View(model);
        }
    }
}