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
using Microsoft.AspNet.Identity;
using System.IO;

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
            if (id == null) return RedirectToAction("Index", controllerName: "Store");
            var prod = db.Products
                .Where(m => m.CategoryId == (int)id)
                .Include(m => m.Specifications)
                .ToList();
            if (prod == null) return RedirectToAction("Index", controllerName: "Store");

            var model = new ProductIndexViewModel { Products = prod };

            //for (int j=0; j < model.Products.Count; j++)
            //{ 
            //    for (int i = 0; i < atrs.Count; i++)
            //    {
            //        if (model.Products.ElementAt(j).SpecificationsDict == null)
            //            model.Products.ElementAt(j).SpecificationsDict = new Dictionary<string, Specification>();
            //        model.Products.ElementAt(j).SpecificationsDict.Add(
            //            atrs.ElementAt(i),
            //            model.Products.ElementAt(j).Specifications.ElementAt(i)
            //        );
            //    }
            //}
            var store = (db.Categories.FirstOrDefault(m => m.Id == id));
            int storeID = 0;
            if (store != null) storeID = store.StoreId;
            ViewBag.catId = id;
            ViewBag.storeId = storeID;

            ViewBag.Flag = true;
            if (GetVendorProductsCount() >= GetMaxProductsAllowed())
            {
                ViewBag.Flag = false;
            }

            return View(model);
        }

        public ActionResult Create(int? catId)
        {
            if (GetVendorProductsCount() >= GetMaxProductsAllowed())
            {
                return View("Error");
            }
            if (catId == null) return RedirectToAction("Index", controllerName: "Store");
            TempData["catId"] = catId;
            var parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == catId);
            var model = new ProductCreateViewModel {
                Specifications = new Dictionary<string, string>(),
                IsActive = true,
                DefaultGST = parent.DefaultGST
            };
            model.Attributes = new Dictionary<string, int>();
            while (parent != null)
            {
                foreach (var atr in parent.Attributes.Reverse())
                {
                    model.Attributes.Add(atr.Name, atr.Id);
                    model.Specifications.Add(atr.Name, atr.Default);
                }
                parent = db.Categories.Include(m => m.Attributes).FirstOrDefault(m => m.Id == parent.CategoryId);
            }
            model.AttributeNames = model.Attributes.Keys.ToList();
            model.AttributeNames.Reverse();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCreateViewModel model)
        {
            int? catId = TempData["catId"] as int?;
            if (catId == null) return RedirectToAction("Index", controllerName: "Store");
            
            if (ModelState.IsValid)
            {
                var prod = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Manufacturer = model.Manufacturer,
                    CategoryId = (int)catId,
                    Specifications = new List<Specification>(),
                    DateAdded = DateTime.Now,
                    IsActive = model.IsActive,
                    DefaultGST = model.DefaultGST
                };

                if (model.Specifications == null) model.Specifications = new Dictionary<string, string>();
                foreach (var atr in model.Specifications.Keys)
                {
                    prod.Specifications.Add(new Specification { Name = atr, Value = model.Specifications[atr] });
                }

                db.Products.Add(prod);
                db.SaveChanges();

                return RedirectToAction("Index", new { id = catId });
            }

            TempData["catId"] = catId;
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return View("Error");

            var prod = db.Products
                .Include(m => m.Category)
                .Include(m => m.Specifications)
                .FirstOrDefault(m => m.Id == id);
            if (prod == null) return View("Error");

            var model = new ProductEditViewModel
            {
                Id = prod.Id,
                Name = prod.Name,
                Description = prod.Description,
                Manufacturer = prod.Manufacturer,
                DateAdded = prod.DateAdded,
                AttributeNames = new List<string>(),
                Attributes = new Dictionary<string, int>(),
                Specifications = new Dictionary<string, string>(),
                IsActive = prod.IsActive,
                ThumbnailPath = prod.ThumbnailPath,
                ThumbnailMimeType = prod.ThumbnailMimeType
            };

            var catIds = new List<int>();
            var cat = prod.Category;
            while(cat != null)
            {
                catIds.Add(cat.Id);
                cat = db.Categories.Include(m => m.Parent).FirstOrDefault(m => m.Id == cat.CategoryId);
            }
            
            foreach(var spec in prod.Specifications)
            {
                var atr = db.Attributes.FirstOrDefault(m => catIds.Contains(m.CategoryId) && m.Name == spec.Name);
                if (atr == null) return View("Error");
                model.AttributeNames.Add(atr.Name);
                model.Attributes.Add(atr.Name, atr.Id);
                model.Specifications.Add(spec.Name, spec.Value);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductEditViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var specs = db.Specifications.Where(m => m.ProductId == model.Id).ToList();
                foreach (var spec in specs)
                {
                    db.Specifications.Remove(spec);
                }

                var prod = db.Products.Include(m => m.Category).FirstOrDefault(m => m.Id == model.Id);
                prod.Name = model.Name;
                prod.Manufacturer = model.Manufacturer;
                prod.Description = model.Description;
                prod.DateAdded = model.DateAdded;
                prod.Specifications = new List<Specification>();
                prod.IsActive = model.IsActive;

                if (prod == null) return View("Error");
                
                List<string> keys;
                if (model.Specifications == null || model.Specifications.Keys == null)
                {
                    keys = new List<string>();
                }
                else
                {
                    keys = model.Specifications.Keys.ToList();
                }
                foreach (var atr in keys)
                {
                    if(string.IsNullOrEmpty(model.Specifications[atr]))
                    {
                        model.Specifications[atr] = "";
                    }
                    var spec = db.Specifications.Add(
                        new Specification
                        {
                            Name = atr,
                            Value = model.Specifications[atr],
                            ProductId = prod.Id
                        });
                    prod.Specifications.Add(spec);
                }

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase hpf = Request.Files["thumbnail"];
                    if (hpf.ContentLength != 0)
                    {
                        if (prod.ThumbnailPath != null
                            && prod.ThumbnailMimeType == null)
                            System.IO.File.Delete(prod.ThumbnailPath);

                        prod.ThumbnailMimeType = hpf.ContentType;
                        prod.ThumbnailPath = System.Web.HttpContext.Current.Server
                                .MapPath("~/App_Data/Images");
                        System.IO.Directory.CreateDirectory(prod.ThumbnailPath);
                        prod.ThumbnailPath += "/Products/";
                        System.IO.Directory.CreateDirectory(prod.ThumbnailPath);
                        prod.ThumbnailPath += User.Identity.GetUserName() + "/";
                        System.IO.Directory.CreateDirectory(prod.ThumbnailPath);
                        prod.ThumbnailPath += prod.Category.Name + "/";
                        System.IO.Directory.CreateDirectory(prod.ThumbnailPath);
                        prod.ThumbnailPath += prod.Name + "/";
                        System.IO.Directory.CreateDirectory(prod.ThumbnailPath);
                        prod.ThumbnailPath += DateTime.Now.Ticks + "_" + hpf.FileName; 
                        var byteBuff = new byte[hpf.ContentLength];
                        hpf.InputStream.Read(byteBuff, 0, hpf.ContentLength);
                        System.IO.File.WriteAllBytes(prod.ThumbnailPath, byteBuff);
                        //prod.ThumbnailData = new byte[hpf.ContentLength];
                        //hpf.InputStream.Read(prod.ThumbnailData, 0, hpf.ContentLength);
                    }

                    Upload(prod.Id, files);
                }

                db.Entry(prod).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }
        
        public JsonResult Upload(int id, IEnumerable<HttpPostedFileBase> files)
        {
            if(id == 0) return Json(new { result = false });

            var prod = db.Products
                .Include(m => m.Category)
                .FirstOrDefault(m => m.Id == id);

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = System.Web.HttpContext.Current.Server
                                    .MapPath("~/App_Data/Images");
                        System.IO.Directory.CreateDirectory(path);
                        path += "/Products/";
                        System.IO.Directory.CreateDirectory(path);
                        path += User.Identity.GetUserName() + "/";
                        System.IO.Directory.CreateDirectory(path);
                        path += prod.Category.Name + "/";
                        System.IO.Directory.CreateDirectory(path);
                        path += prod.Name + "/";

                        path += DateTime.Now.Ticks + "_" + file.FileName;

                        file.SaveAs(path);

                        db.ProductImages.Add(
                            new ProductImage
                            {
                                ImageMimeType = file.ContentType,
                                ImagePath = path,
                                ProductId = id
                            }
                        );
                    }
                }
            }
            db.SaveChanges();
            return Json(new { result = true });
        }

        [AllowAnonymous]
        public FileContentResult GetThumbnail(int? id)
        {
            if (id == null) return null;
            Product prod = db.Products.FirstOrDefault(m => m.Id == id);
            if (prod == null) return null;
            if (prod.ThumbnailPath == null || prod.ThumbnailMimeType == null)
                return null;
            return File(System.IO.File.ReadAllBytes(prod.ThumbnailPath), prod.ThumbnailMimeType);
        }

        [AllowAnonymous]
        public FileContentResult GetImage(string path, string type)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (path == null || type == null)
                return null;
            return File(System.IO.File.ReadAllBytes(path), type);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return View("Error");
            var model = db.Products
                .Include(m => m.Category)
                .FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product model)
        {
            if (model.Id != 0)
            {
                var entry = db.Entry(model);
                if (entry.State == EntityState.Detached)
                    db.Products.Attach(model);
                var stock = db.Stocks.FirstOrDefault(m => m.ProductId == model.Id);
                if(stock != null)
                {
                    var stockEntry = db.Entry(stock);
                    if (stockEntry.State == EntityState.Detached)
                        db.Stocks.Attach(stock);
                    db.Stocks.Remove(stock);
                }
                db.Products.Remove(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = model.CategoryId });
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Stock(int? prodId, int? storeId)
        {
            if (prodId == null || storeId == null) return RedirectToAction("Index", controllerName: "Store");
            TempData["prodId"] = prodId;
            TempData["storeId"] = storeId;
            var oldStock = db.Stocks.FirstOrDefault(m => m.ProductId == prodId && m.StoreId == storeId);
            var prod = db.Products.FirstOrDefault(m => m.Id == prodId);
            var model = new ProductStockViewModel {
                Product = prod,
                MaxPerUser = 10,
                GST = prod.DefaultGST
            };
            if (oldStock != null)
            {
                model.Price = oldStock.Price;
                model.Stock = oldStock.CurrentStock;
                model.MaxPerUser = oldStock.MaxAmtPerUser;
                model.GST = oldStock.GST;
            }
            if (model.Product == null) return RedirectToAction("Index", controllerName: "Store");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stock(ProductStockViewModel model)
        {
            int? prodId = TempData["prodId"] as int?;
            int? storeId = TempData["storeId"] as int?;
            if (prodId == null || storeId == null) return RedirectToAction("Index", controllerName: "Store");
            var oldStock = db.Stocks.FirstOrDefault(m => m.ProductId == prodId && m.StoreId == storeId);
            if (ModelState.IsValid)
            {
                if (model.Stock <= 0) model.Status = StockStatus.OutOfStock;
                else model.Status = StockStatus.InStock;
                if (oldStock is null)
                {
                    var stock = db.Stocks.Add(
                        new Stock
                        {
                            CurrentStock = model.Stock,
                            Price = model.Price,
                            ProductId = (int)prodId,
                            StoreId = (int)storeId,
                            StockingDate = DateTime.Now,
                            Status = model.Status,
                            MaxAmtPerUser = model.MaxPerUser,
                            GST = model.GST
                        }
                    );
                }
                else
                {
                    oldStock.CurrentStock = model.Stock;
                    oldStock.Price = model.Price;
                    oldStock.Status = model.Status;
                    oldStock.MaxAmtPerUser = model.MaxPerUser;
                    oldStock.GST = model.GST;
                    db.Entry(oldStock).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index", controllerName: "Store");
            }

            TempData["prodId"] = prodId;
            TempData["storeId"] = storeId;
            var stock_2 = db.Stocks
                .Include(m => m.Product)
                .FirstOrDefault(m => m.Id == model.Stock);
            model = new ProductStockViewModel {
                Stock = stock_2.Id,
                Price = stock_2.Price,
                MaxPerUser = stock_2.MaxAmtPerUser,
                Product = stock_2.Product
            };
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return View("Error");
            var model = db.Products
                .Include(m => m.Category)
                .Include(m => m.Specifications)
                .Include(m => m.ProductImages)
                .FirstOrDefault(m => m.Id == id);
            if (model == null) return View("Error");
            return View(model);
        }

        public ActionResult ChangeCategory(int? id)
        {
            if (id == null) return View("Error");
            var prod = db.Products
                .Include(m => m.Category)
                .FirstOrDefault(m => m.Id == id);
            if (prod == null) return View("Error");
            var possibleCats = db.Categories
                .Where(m => db.Categories.Where(p => p.CategoryId == m.Id).FirstOrDefault() == null)
                .ToList();

            var possibleCatNames = possibleCats
                .Where(m => m.StoreId == prod.Category.StoreId && m.Id != prod.CategoryId)
                .Select(m => m.Name)
                .ToList();

            var model = new ProductChangeCategoryViewModel
            {
                Id = prod.Id
            };

            ViewBag.Name = prod.Name;
            ViewBag.Current = prod.Category.Name;
            ViewBag.Cats = possibleCatNames;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCategory(ProductChangeCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var prod = db.Products
                    .Include(m => m.Specifications)
                    .FirstOrDefault(m => m.Id == model.Id);
                if (prod == null) return View("Error");

                var cat = db.Categories
                    .Include(m => m.Attributes)
                    .FirstOrDefault(m => m.Name == model.SelectedCategory);
                prod.CategoryId = cat.Id;
                prod.Category = cat;

                db.Entry(prod).State = EntityState.Modified;

                db.SaveChanges();
                if (true)
                {
                    foreach(var spec in db.Specifications.Where(m => m.ProductId == prod.Id).ToList())
                    {
                        db.Specifications.Remove(spec);
                    }
                    var allAtrs = cat.Attributes.ToList();
                    allAtrs.Reverse();
                    var parent = db.Categories
                        .Include(m => m.Attributes)
                        .FirstOrDefault(m => cat.CategoryId == m.Id);

                    while(parent != null)
                    {
                        var newAtrs = parent.Attributes.ToList();
                        newAtrs.Reverse();
                        foreach (var atr in newAtrs)
                            allAtrs.Add(atr);
                        parent = db.Categories
                        .Include(m => m.Attributes)
                        .FirstOrDefault(m => parent.CategoryId == m.Id);
                    }
                    allAtrs.Reverse();
                    foreach(var atr in allAtrs)
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

                return RedirectToAction("Details", new { id = model.Id });
            }

            return RedirectToAction("ChangeCategory", new { id = model.Id });
        }

        public ActionResult AllProducts()
        {
            var userId = User.Identity.GetUserId();
            var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (user is null) return View("Error");
            var prods = db.Products
                .Include(m => m.Category)
                .Include(m => m.Category.Store)
                .Where(m => m.Category.Store.EcomUserId == user.Id)
                .ToList();

            var model = new ProductAllViewModel
            {
                Products = prods,
                Inventory = new Dictionary<Product, List<Stock>>(),
                HasListing = new Dictionary<Product, bool>()
            };

            foreach (var prod in prods)
            {
                model.Inventory.Add(prod,
                    db.Stocks
                    .Include(m => m.Store)
                    .Where(m => m.ProductId == prod.Id)
                    .ToList());

                if (db.Categories.FirstOrDefault(m => m.CategoryId == prod.CategoryId) == null)
                {
                    model.HasListing.Add(prod, true);
                }
                else
                {
                    model.HasListing.Add(prod, false);
                }
            }
            return View(model);
        }

        public ActionResult EditActiveProducts()
        {
            var userId = User.Identity.GetUserId();
            var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (user is null) return View("Error");
            var prods = db.Products
                .Include(m => m.Category)
                .Include(m => m.Category.Store)
                .Where(m => m.Category.Store.EcomUserId == user.Id)
                .ToList();

            var model = new ProductEditListedViewModel
            {
                Products = prods,
                Inventory = new Dictionary<Product, List<Stock>>(),
                IsActiveList = new Dictionary<int, bool>(),
                HasListing = new Dictionary<Product, bool>(),
                CurrentSelectedProducts = GetActiveProductsCount(),
                MaxAllowedProducts = GetMaxProductsAllowed()
            };

            foreach (var prod in prods)
            {
                model.IsActiveList.Add(prod.Id, prod.IsActive);

                model.Inventory.Add(prod,
                    db.Stocks
                    .Include(m => m.Store)
                    .Where(m => m.ProductId == prod.Id)
                    .ToList());

                if (db.Categories.FirstOrDefault(m => m.CategoryId == prod.CategoryId) == null)
                {
                    model.HasListing.Add(prod, true);
                }
                else
                {
                    model.HasListing.Add(prod, false);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditActiveProducts(ProductEditListedViewModel model, string[] actives)
        {
            if (ModelState.IsValid)
            {
                var prodIds = Request.Form["prodIds"];
                var activeProds = actives ?? new string[0];
                if (actives == null) actives = new string[0];

                var userId = User.Identity.GetUserId();

                var user = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == userId);
                if (user is null) return View("Error");
                var prods = db.Products
                .Include(m => m.Category)
                .Include(m => m.Category.Store)
                .Where(m => m.Category.Store.EcomUserId == user.Id)
                .ToList();

                foreach (var prod in prods)
                {
                    if (actives.Contains(prod.Id.ToString()))
                        prod.IsActive = true;
                    else
                        prod.IsActive = false;
                    db.Entry(prod).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("AllProducts");
            }
            return RedirectToAction("EditActiveProducts");
        }

        public ActionResult EditDetailsImages(int? id)
        {
            if (id == null) return View("Error");
            var model = new ProductEditImagesViewModel
            {
                ProductImages = db.ProductImages
                                    .Where(m => m.ProductId == id)
                                    .ToList(),
                ProductId = (int) id
            };

            if (model.ProductImages == null)
                model.ProductImages = new List<ProductImage>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetailsImages(int? id, int? prodId)
        {
            if (id == null || prodId == null) return View("Error");

            var prodImg = db.ProductImages.FirstOrDefault(m => m.Id == id);

            System.IO.File.Delete(prodImg.ImagePath);

            db.Entry(prodImg).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("EditDetailsImages", new { id = prodId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductImageUpload(HttpPostedFileBase file, int prodId)
        {
            if (file.ContentLength == 0) return View("Error");

            try
            {
                var prod = db.Products
                             .Include(m => m.Category)
                             .Include(m => m.ProductImages)
                             .FirstOrDefault(m => m.Id == prodId);

                if (prod == null)
                    return View("Error");

                else if (prod.ProductImages.Count >= 6)
                {
                    ViewBag.Msg = "Max limit of 6 reached.";
                    throw new Exception("Max limit of 6 reached.");
                }
                
                var memStream = new MemoryStream();
                file.InputStream.CopyTo(memStream);

                byte[] fileData = memStream.ToArray();

                var prodImg = new ProductImage
                {
                    ImageMimeType = file.ContentType,
                    ImagePath = System.Web.HttpContext.Current.Server
                                .MapPath("~/App_Data/Images"),
                    ProductId = prodId,
                    Product = prod
                };

                System.IO.Directory.CreateDirectory(prodImg.ImagePath);
                prodImg.ImagePath += "/Products/";
                System.IO.Directory.CreateDirectory(prodImg.ImagePath);
                prodImg.ImagePath += User.Identity.GetUserName() + "/";
                System.IO.Directory.CreateDirectory(prodImg.ImagePath);
                prodImg.ImagePath += prod.Category.Name + "/";
                System.IO.Directory.CreateDirectory(prodImg.ImagePath);
                prodImg.ImagePath += prod.Name + "/";
                System.IO.Directory.CreateDirectory(prodImg.ImagePath);
                prodImg.ImagePath += DateTime.Now.Ticks + "_" + file.FileName;

                file.SaveAs(prodImg.ImagePath);

                db.ProductImages.Add(prodImg);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
                return Json(new
                {
                    success = false,
                    response = ex.Message
                });
            }

            ViewBag.Msg = "File uploaded.";
            return Json(new { success = true,
                              response = "File uploaded."});
        }

        private int GetActiveProductsCount()
        {
            var vendorId = User.Identity.GetUserId();
            var vendor = db.EcomUsers
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.ApplicationUserId == vendorId);

            var prods = db.Products
                        .Include(m => m.Category)
                        .Include(m => m.Category.Store)
                        .Where(m => m.Category.Store.EcomUserId == vendor.Id 
                                    && m.IsActive == true)
                        .ToList();

            return prods.Count;
        }

        private int GetVendorProductsCount()
        {
            var vendorId = User.Identity.GetUserId();
            var vendor = db.EcomUsers
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.ApplicationUserId == vendorId);

            var prods = db.Products
                        .Include(m => m.Category)
                        .Include(m => m.Category.Store)
                        .Where(m => m.Category.Store.EcomUserId 
                                == vendor.Id)
                        .ToList();

            return prods.Count;
        }

        private int GetMaxProductsAllowed()
        {
            var vendorId = User.Identity.GetUserId();
            var vendor = db.EcomUsers
                        .Include(m => m.VendorDetails)
                        .Include(m => m.VendorDetails.ActivePlan)
                        .FirstOrDefault(m => m.ApplicationUserId == vendorId);

            var pkg = db.VendorPlans
                        .FirstOrDefault(m => m.Id ==
                        vendor.VendorDetails.ActivePlan.VendorPlanId);

            return pkg.MaxProducts;
        }
    }
}