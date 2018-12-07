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
using System.IO;
using Microsoft.AspNet.Identity;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class CategoryController : Controller
    {
        public CategoryController()
        {
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Category
        public ActionResult Index(int? id)
        {
            var userId = User.Identity.GetUserId();
            var ecomUser = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (id == null)
                return RedirectToAction("Index", controllerName: "Store");
            if (ecomUser == null || id != ecomUser.Id)
            {
                return RedirectToAction("Index", new { id = ecomUser.Id });
            }
            ViewBag.storeId = (int)id;
            var model = db.Categories.Include(m => m.Attributes).Where(m => m.StoreId == (int)id).ToList();
            return View(model);
        }
        
        public ActionResult AddAttributePartial(int? id)
        {
            if (id == null) return View("Error");
            var model = new AddAttributeViewModel() { Type = InformationType.Other, Default = "" };
            return PartialView("AddAttributePartial", model);
        }

        public ActionResult Create(int? storeId, int? catId)
        {
            var dictCat = new Dictionary<string, int>();
            foreach (var cat in db.KL_Categories.ToList())
                dictCat.Add(cat.Name, cat.Id);

            TempData["storeId"] = storeId;
            TempData["catId"] = catId;
            ViewBag.StoreId = storeId;
            
            return View(new CategoryCreateViewModel {
                DefaultGST = 10.0f,
                Categories = dictCat
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryCreateViewModel model)
        {
            int? storeId = TempData["storeId"] as int?;
            int? catId = TempData["catId"] as int?;
            if (storeId == null) return RedirectToAction("Index", controllerName: "Store");

            if(ModelState.IsValid)
            {
                List<CategoryAttribute> catAttrs = new List<CategoryAttribute>();
                var nameStr = Request.Form["AtrbName"];
                var typesStr = Request.Form["Type"];
                var defaultStr = Request.Form["Default"];

                if (nameStr is null) nameStr = "";
                var atrbNames = nameStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (atrbNames is null) atrbNames = new string[] { };

                if (typesStr is null) typesStr = "";
                var typeNames = typesStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (typeNames is null) typeNames = new string[] { };

                if (defaultStr is null) defaultStr = "";
                var dfltNames = defaultStr.Split(',').Select(sValue => sValue.Trim()).ToArray() as string[];
                if (dfltNames is null) dfltNames = new string[] { };

                for (int i = 0; i < atrbNames.Count(); i++)
                {
                    if (string.IsNullOrEmpty(dfltNames[i])) dfltNames[i] = "";
                    if (atrbNames[i] != "")
                        catAttrs.Add(new CategoryAttribute()
                        {
                            Name = atrbNames[i],
                            Default = dfltNames[i],
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
                    DefaultGST = model.DefaultGST,
                    CategoryId = catId,
                    StoreId = (int)storeId,
                    KL_CategoryId = model.SelectedCat
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
            if (storeId == null || catId == null || storeId == 0 || catId == 0) return RedirectToAction("Index", controllerName: "Store");
            ViewBag.storeId = storeId;  
            var cat = db.Categories.Include(m => m.Attributes)
                .Include(m => m.KL_Category)
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
            if (cat == null) return RedirectToAction("Index", controllerName: "Store");

            return View(new CategoryDetailsViewModel
            {
                Category = cat,
                HasThumbnail = (cat.ThumbnailData != null && cat.ThumbnailMimeType != null) ? true : false
            });
        }

        [AllowAnonymous]
        public FileContentResult GetThumbnail(int? id)
        {
            if (id == null) return null;
            Category cat = db.Categories.FirstOrDefault(m => m.Id == id);
            if (cat == null) return null;
            try
            {
                return File(cat.ThumbnailData, cat.ThumbnailMimeType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return View("Error");
            var cat = db.Categories
                .Include(m => m.Attributes)
                .FirstOrDefault(m => m.Id == id);
            if (cat == null) return View("Error");
            if (cat.Attributes == null) cat.Attributes = new List<CategoryAttribute>();

            var dictCat = new Dictionary<string, int>();
            foreach (var kl_cat in db.KL_Categories.ToList())
                dictCat.Add(kl_cat.Name, kl_cat.Id);

            return View(new CategoryEditViewModel {
                Id = cat.Id,
                StoreId = cat.StoreId,
                Name = cat.Name,
                Description = cat.Description,
                Attributes = cat.Attributes.ToList(),
                SelectedCat = cat.KL_CategoryId,
                Categories = dictCat,
                DefaultGST = cat.DefaultGST
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryEditViewModel model, string[] AtrbName,
            string[] Type, string[] Default)
        {
            if (model == null) return View("Error");

            if (Request.Files.Count > 0)
            {
                if (Request.Files.Count != 1) return View("Error");
                HttpPostedFileBase hpf = Request.Files["thumbnail"];

                if (hpf.ContentLength > 5000000)
                {
                    ModelState.AddModelError("thumbnail", "Thumbnail cannot be greater than 5mb.");
                }
            }

            if (ModelState.IsValid)
            {
                var atrbs = db.Attributes.Where(s => s.CategoryId == model.Id).ToList();
                foreach (var attrb in atrbs)
                {
                    db.Attributes.Remove(attrb);
                }   

                List<CategoryAttribute> catAttrs = new List<CategoryAttribute>();
                
                var atrbNames = AtrbName;
                if (atrbNames is null) atrbNames = new string[] { };
                
                var typeNames = Type;
                if (typeNames is null) typeNames = new string[] { };

                var dfltNames = Default;
                if (dfltNames is null) dfltNames = new string[] { };

                for (int i=0; i<atrbNames.Count(); i++)
                {
                    if (atrbNames[i] != "")
                        catAttrs.Add(new CategoryAttribute() {
                            Name = atrbNames[i],
                            InfoType = (InformationType) Enum.Parse(typeof(InformationType), typeNames[i]),
                            Default = dfltNames[i],
                            CategoryId = model.Id });
                }


                //foreach (var typeName in typeNames)
                //{
                //    if (typeName != "")
                //        catAttrs.Add(new CategoryAttribute() { Name = atrbName, CategoryId = model.Id });
                //}

                var cat = db.Categories.FirstOrDefault(m => m.Id == model.Id);
                if (cat == null) return View("Error");



                foreach (var atrb in catAttrs)
                {
                    if (model.Attributes == null) model.Attributes = new List<CategoryAttribute>();
                    db.Attributes.Add(atrb);
                    cat.Attributes.Add(atrb);
                }

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.KL_CategoryId = model.SelectedCat;

                if(Request.Files.Count > 0)
                {
                    if (Request.Files.Count != 1) return View("Error");
                    HttpPostedFileBase hpf = Request.Files["thumbnail"];
                    if (hpf != null && hpf.ContentLength != 0)
                    {
                        cat.ThumbnailMimeType = hpf.ContentType;
                        cat.ThumbnailData = new byte[hpf.ContentLength];
                        hpf.InputStream.Read(cat.ThumbnailData, 0, hpf.ContentLength);
                    }
                }

                db.Entry(cat).State = EntityState.Modified;
                
                try
                {
                    db.SaveChanges();
                    if(true) UpdateProducts(model.Id, atrbs.ToList());
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
                if (true)
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
                            Value = atr.Default,
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
                    if (prod.Specifications.FirstOrDefault(m => m.Name == newAtr.Name) == null)
                    {
                        prod.Specifications.Add(db.Specifications.Add(new Specification { Name = newAtr.Name, Value = newAtr.Default, ProductId = prod.Id }));
                    }
                }
                foreach (var oldAtr in oldAtrs)
                {
                    var atrb = db.Specifications.FirstOrDefault(m => m.ProductId == prod.Id
                        && m.Name == oldAtr.Name);
                    if (atrb != null) db.Specifications.Remove(atrb);
                }
                db.Entry(prod).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        
        private List<CategoryAttribute> AttrsToBeAdded(List<CategoryAttribute> attrs, List<CategoryAttribute> oldAttrs)
        {
            var newAtrs = new List<CategoryAttribute>();
            foreach(var atr in attrs)
            {
                var oldAtrb = oldAttrs.Where(m => m.Name == atr.Name).FirstOrDefault();
                if (oldAtrb == null) newAtrs.Add(atr);
                else if(oldAtrb.Default != atr.Default)
                    newAtrs.Add(atr);
            }
            return newAtrs;
        }

        private List<CategoryAttribute> AttrsToBeRemoved(List<CategoryAttribute> attrs, List<CategoryAttribute> oldAttrs)
        {
            var dltAtrs = new List<CategoryAttribute>();
            foreach (var atr in oldAttrs)
            {
                var atrb = attrs.Where(m => m.Name == atr.Name).FirstOrDefault();
                if (atrb == null) dltAtrs.Add(atr);
                else if (atrb.Default != atr.Default)
                    dltAtrs.Add(atr);
            }
            return dltAtrs;
        }
    }
}