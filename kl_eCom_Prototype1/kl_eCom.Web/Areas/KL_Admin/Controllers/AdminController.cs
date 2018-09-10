using kl_eCom.Web.Areas.KL_Admin.Models;
using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Entities;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: KL_Admin/Admin
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName == "klAdmin" 
                        && model.Email == "khushlife@gmail.com"
                        && model.Password == "KLadmin@123")
                {
                    var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

                    var result = signInManager.PasswordSignIn(model.UserName, model.Password, false, false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                        {
                            return RedirectToAction("Home");
                        }
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            return RedirectToAction("Index");
        }
        
        public ActionResult Home()
        {
            var model = new AdminHomeViewModel
            {
                VendorPackages = db.VendorPlans.ToList()
            };
            return View(model);
        }

        public ActionResult Plans()
        {
            return View(new AdminPlansIndexViewModel {
                VendorPackages = db.VendorPlans.ToList()
            });
        }

        public ActionResult Categories()
        {
            var allCategories = db.KL_Categories
                .Include(m => m.ChildCategories)
                .ToList();

            var baseCategories = allCategories.Where(m => m.KL_CategoryId == null).ToList();

            var categoryDict = new Dictionary<KL_Category, List<KL_Category>>();

            foreach (var cat in allCategories)
            {
                if (cat.ChildCategories != null && cat.ChildCategories.Count > 0)
                {
                    categoryDict.Add(cat, cat.ChildCategories.ToList());
                }
                else
                {
                    categoryDict.Add(cat, new List<KL_Category>());
                }
            }
            return View(new AdminCategoriesViewModel {
                BaseCategories = baseCategories,
                ChildCategories = categoryDict
            }); 
        }

        public ActionResult CreateCategory(string id = "")
        {
            var allCategories = db.KL_Categories
                .Include(m => m.ChildCategories)
                .OrderBy(m => m.Name)
                .ToList();

            

            var model = new AdminCategoryCreateViewModel {
                Categories = new Dictionary<string, int>(),
                ParentId = 0
            };

            foreach (var cat in allCategories)
            {
                model.Categories.Add(cat.Name, cat.Id);
            }

            ViewBag.Message = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(AdminCategoryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.KL_Categories.Add(new KL_Category {
                    Name = model.Name,
                    KL_CategoryId = model.ParentId
                });
                db.SaveChanges();

                return RedirectToAction("Categories");
            }

            return RedirectToAction("CreateCategory", new { msg = "Error in creating." });
        }

        public ActionResult CategoryDetails(int? id)
        {
            if (id == null) return View("Error");
            return View(db.KL_Categories.Include(m => m.ParentCategory).FirstOrDefault(m => m.Id == id));
        }

        public ActionResult CategoryDelete(int? id)
        {
            if (id == null) return View("Error");
            return View(db.KL_Categories.Include(m => m.ParentCategory).FirstOrDefault(m => m.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryDelete(KL_Category model)
        {
            if (model.KL_CategoryId != null 
                    && model.KL_CategoryId != 0)
            {
                var category = db.KL_Categories.FirstOrDefault(m => m.Id == model.Id);
                if (category is null) return View("Error");
                db.Entry(category).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Categories");
            }

            return RedirectToAction("Categories");
        }

        public ActionResult CategoryEdit(int? id)
        {
            if (id == null) return View("Error");

            var cat = db.KL_Categories
              .FirstOrDefault(m => m.Id == id);
            if (cat is null) return View("Error");

            var model = new AdminCategoryEditViewModel {
                CatId = cat.Id,
                Name = cat.Name,
                ParentId = cat.KL_CategoryId,
                Categories = new Dictionary<string, int>()
            };

            foreach (var catg in db.KL_Categories.Where(m => m.Id != id).ToList())
            {
                model.Categories.Add(catg.Name, catg.Id);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(AdminCategoryEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ParentId == model.CatId) return View("Error");

                var cat = db.KL_Categories.FirstOrDefault(m => m.Id == model.CatId);
                if (cat is null) return View("Error");

                cat.Name = model.Name;
                cat.KL_CategoryId = model.ParentId;

                db.Entry(cat).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("CategoryDetails", new { id = model.CatId });
            }

            return View(model);
        }

        public ActionResult MissingCategories()
        {
            var model = new AdminCategoryMissingViewModel {
                AvailableCategories = new Dictionary<string, int>()
            };

            model.MissingCategories = db.Categories
                .Include(m => m.KL_Category)
                .Where(m => m.KL_Category.Name == "Other")
                .Select(m => m.Name)
                .Distinct()
                .ToList();

            foreach (var cat in db.KL_Categories.ToList())
            {
                model.AvailableCategories.Add(cat.Name, cat.Id);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MissingCategories(AdminCategoryMissingViewModel model, string[] selections)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedCategoryId == 0 || selections == null)
                    return View("Error");

                var catList = new List<Category>();

                foreach (var catName in selections)
                {
                    foreach (var cat in db.Categories.Where(m => m.Name == catName).ToList())
                        catList.Add(cat);
                }

                foreach (var cat in catList)
                {
                    cat.KL_CategoryId = model.SelectedCategoryId;
                    db.Entry(cat).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return RedirectToAction("MissingCategories");
        }

        public ActionResult AllocateCategories(string id = "", bool? isPositive = null)
        {
            var model = new AdminCategoryAllocateViewModel
            {
                AvailableCategories = new Dictionary<string, int>()
            };

            model.VendorCategories = db.Categories
                .Select(m => m.Name)
                .Distinct()
                .ToList();

            foreach (var cat in db.KL_Categories.ToList())
            {
                model.AvailableCategories.Add(cat.Name, cat.Id);
            }

            ViewBag.Message = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AllocateCategories(AdminCategoryAllocateViewModel model, string[] selections)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedCategoryId == 0 || selections == null)
                    return View("Error");

                var catList = new List<Category>();

                foreach (var catName in selections)
                {
                    foreach (var cat in db.Categories.Where(m => m.Name == catName).ToList())
                        catList.Add(cat);
                }

                foreach (var cat in catList)
                {
                    cat.KL_CategoryId = model.SelectedCategoryId;
                    db.Entry(cat).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("AllocateCategories", new { id = "Success!" });
            }
            return RedirectToAction("AllocateCategories", new { id = "Operation Failed." });
        }
    }
}