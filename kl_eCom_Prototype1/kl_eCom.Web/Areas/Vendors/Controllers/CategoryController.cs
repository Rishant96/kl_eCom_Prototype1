﻿using kl_eCom.Web.Models;
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
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Category
        public ActionResult Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", controllerName: "Home");
            ViewBag.storeId = (int)id;
            var model = db.Categories.Where(m => m.StoreId == (int)id).Include(m => m.Attributes).ToList();
            return View(model);
        }
        
        public ActionResult AddAttributePartial(int? id)
        {
            if (id == null) return View("Error");
            var model = new AddAttributeViewModel() { Type = InformationType.Other };
            return PartialView("AddAttributePartial", model);
        }

        public ActionResult Create(int? storeId, int? catId)
        {
            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            ViewBag.StoreId = storeId;
            return View(new CategoryCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryCreateViewModel model)
        {
            int? storeId = TempData["storeId"] as int?;
            int? catId = TempData["catId"] as int?;
            if (storeId == null) return RedirectToAction("Index", controllerName: "Home");

            if(ModelState.IsValid)
            {
                List<CategoryAttribute> catAttrs = new List<CategoryAttribute>();
                var nameStr = Request.Form["AtrbName"];
                var typesStr = Request.Form["Type"];

                if (nameStr is null) nameStr = "";
                var atrbNames = nameStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (atrbNames is null) atrbNames = new string[] { };

                if (typesStr is null) typesStr = "";
                var typeNames = typesStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (typeNames is null) typeNames = new string[] { };

                for (int i = 0; i < atrbNames.Count(); i++)
                {
                    if (atrbNames[i] != "")
                        catAttrs.Add(new CategoryAttribute()
                        {
                            Name = atrbNames[i],
                            InfoType = (InformationType)Enum.Parse(typeof(InformationType), typeNames[i])
                        });
                }

                foreach (var atrb in catAttrs)
                {
                    if (model.Attributes == null) model.Attributes = new List<CategoryAttribute>();
                    model.Attributes.Add(atrb);
                }
                
                var cat = new Category
                {
                    Name = model.Name,
                    IsBase = (catId == null) ? true : false,
                    Description = model.Description,
                    Attributes = model.Attributes,
                    CategoryId = catId,
                    StoreId = (int)storeId
                };

                db.Categories.Add(cat);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

                return RedirectToAction("Index", new { id = storeId });
            }

            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            return View(model);
        }

        public ActionResult Details(int? storeId, int? catId)
        {
            if (storeId == null || catId == null) return RedirectToAction("Index", controllerName: "Home");
            ViewBag.storeId = storeId;  
            var cat = db.Categories.Include(m => m.Attributes)
                .FirstOrDefault(m => m.Id == (int)catId);
            cat.ChildCategories = db.Categories.Where(m => m.CategoryId == cat.Id).ToList();
            if(cat.ChildCategories == null || cat.ChildCategories.Count == 0)
            {
                ViewBag.isLeaf = true;
            }
            else
            {
                ViewBag.isLeaf = false;
            }
            if (cat == null) return RedirectToAction("Index", controllerName: "Home");
            return View(cat);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return View("Error");
            var cat = db.Categories
                .Include(m => m.Attributes)
                .FirstOrDefault(m => m.Id == id);
            if (cat == null) return View("Error");
            if (cat.Attributes == null) cat.Attributes = new List<CategoryAttribute>();
            return View(new CategoryEditViewModel {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
                Attributes = cat.Attributes.ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryEditViewModel model)
        {
            if (model == null) return View("Error");
            if (ModelState.IsValid)
            {
                var atrbs = db.Attributes.ToList().Where(s => s.CategoryId == model.Id);
                foreach (var attrb in atrbs)
                {
                    db.Attributes.Remove(attrb);
                }   
                db.SaveChanges();

                List<CategoryAttribute> catAttrs = new List<CategoryAttribute>();
                var nameStr = Request.Form["AtrbName"];
                var typesStr = Request.Form["Type"];

                if (nameStr is null) nameStr = "";
                var atrbNames = nameStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (atrbNames is null) atrbNames = new string[] { };

                if (typesStr is null) typesStr = "";
                var typeNames = typesStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (typeNames is null) typeNames = new string[] { };
                
                for (int i=0; i<atrbNames.Count(); i++)
                {
                    if (atrbNames[i] != "")
                        catAttrs.Add(new CategoryAttribute() {
                            Name = atrbNames[i],
                            InfoType = (InformationType) Enum.Parse(typeof(InformationType), typeNames[i]),
                            CategoryId = model.Id });
                }


                //foreach (var typeName in typeNames)
                //{
                //    if (typeName != "")
                //        catAttrs.Add(new CategoryAttribute() { Name = atrbName, CategoryId = model.Id });
                //}

                foreach (var atrb in catAttrs)
                {
                    if (model.Attributes == null) model.Attributes = new List<CategoryAttribute>();
                    db.Attributes.Add(atrb);
                    (db.Categories.FirstOrDefault(m => m.Id == model.Id)).Attributes.Add(atrb);
                }
                
                db.Entry(db.Categories.FirstOrDefault(m => m.Id == model.Id)).State = EntityState.Modified;
                
                try
                {
                    db.SaveChanges();
                    if(model.ReflectChange) UpdateProducts(model.Id, atrbs.ToList());
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

                return RedirectToAction("Details", "Category", new { storeId = model.StoreId, catId = model.Id });
            }
            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if(id == null) return View("Error");
            return View(db.Categories.FirstOrDefault(m => m.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Category model)
        {
            var entry = db.Entry(model);
            if (entry.State == EntityState.Detached)
                db.Categories.Attach(model);
            db.Categories.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = model.StoreId });
        }

        public ActionResult ImportProduct(int? id)
        {
            if (id == null) return View("Error");

            var cat = db.Categories
                .FirstOrDefault(m => m.Id == id);
            if (cat == null) return View("Error");

            var prods = db.Products
                .Include(m => m.Category)
                .ToList();
            prods = prods.Where(m => m.Category.StoreId == cat.StoreId).ToList();

            var possibleProds = prods
                .Where(m => m.CategoryId != cat.Id)
                .Select(m => m.Name)
                .ToList();

            var currentProds = prods
                .Where(m => m.CategoryId == cat.Id)
                .Select(m => m.Name)
                .ToList();

            ViewBag.Name = cat.Name;
            ViewBag.StoreId = cat.StoreId;
            ViewBag.CurrProds = currentProds;
            ViewBag.AvlblProds = possibleProds;

            return View(new CategoryImportProductViewModel {
                Id = cat.Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportProduct(CategoryImportProductViewModel model)
        {
            if (ModelState.IsValid)
            {

                var cat = db.Categories
                    .Include(m => m.Attributes)
                    .FirstOrDefault(m => m.Id == model.Id);

                if (cat == null) return View("Error");

                var prod = db.Products
                    .Include(m => m.Specifications)
                    .FirstOrDefault(m => m.Name == model.ProductName);

                if (prod == null) return View("Error");
                
                prod.CategoryId = cat.Id;
                prod.Category = cat;

                db.Entry(prod).State = EntityState.Modified;

                db.SaveChanges();
                if (model.ReflectChange)
                {
                    foreach (var spec in db.Specifications.Where(m => m.ProductId == prod.Id).ToList())
                    {
                        db.Specifications.Remove(spec);
                    }
                    foreach (var atr in cat.Attributes)
                    {
                        db.Specifications.Add(new Specification
                        {
                            Name = atr.Name,
                            Value = "",
                            ProductId = prod.Id
                        });
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ImportProduct", new { id = model.Id });
        }

        private void UpdateProducts(int id, List<CategoryAttribute> oldAttrs)
        {
            var cat = db.Categories
                .Include(m =>m.Attributes)
                .FirstOrDefault(m => m.Id == id);
            if (cat == null) return;
            
            var newAtrs = AttrsToBeAdded(cat.Attributes.ToList(), oldAttrs);
            var oldAtrs = AttrsToBeRemoved(cat.Attributes.ToList(), oldAttrs);

            var prodCats = new List<Category>();
            var catQueue = new Queue<Category>();
            catQueue.Enqueue(cat);
            while(catQueue.Count != 0)
            {
                var frontCat = catQueue.Dequeue();
                var children = db.Categories.Where(m => m.CategoryId == frontCat.Id).ToList();
                if(children.Count == 0)
                {
                    prodCats.Add(frontCat);
                }
                else
                {
                    foreach(var childCat in children)
                    {
                        catQueue.Enqueue(childCat);
                    }
                }
            }

            var prods = new List<Product>();
            foreach(var pCat in prodCats)
            {
                foreach(var p in db.Products.Include(p => p.Specifications).Where(m => m.CategoryId == pCat.Id).ToList())
                {
                    prods.Add(p);
                }
            }
            
            foreach (var prod in prods)
            {
                foreach (var newAtr in newAtrs)
                {
                    prod.Specifications.Add(db.Specifications.Add(new Specification { Name = newAtr, Value = "", ProductId = prod.Id }));
                }
                foreach (var oldAtr in oldAtrs)
                {
                    db.Specifications.Remove(db.Specifications.FirstOrDefault(m => m.ProductId == prod.Id
                        && m.Name == oldAtr));
                }
                db.Entry(prod).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        
        private List<string> AttrsToBeAdded(List<CategoryAttribute> attrs, List<CategoryAttribute> oldAttrs)
        {
            var newAtrs = new List<string>();
            foreach(var atr in attrs)
            {
                if (oldAttrs.Where(m => m.Name == atr.Name).FirstOrDefault() == null) newAtrs.Add(atr.Name);
            }
            return newAtrs;
        }

        private List<string> AttrsToBeRemoved(List<CategoryAttribute> attrs, List<CategoryAttribute> oldAttrs)
        {
            var dltAtrs = new List<string>();
            foreach (var atr in oldAttrs)
            {
                if (attrs.Where(m => m.Name == atr.Name).FirstOrDefault() == null) dltAtrs.Add(atr.Name);
            }
            return dltAtrs;
        }
    }
}