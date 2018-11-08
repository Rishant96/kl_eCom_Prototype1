using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    public class HomeController : Controller
    {
        #region CTOR + Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion
        
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Home
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Vendor"))
                return RedirectToAction("Index", controllerName: "Vendor");
            return View();
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
                .Select(s => new SelectListItem {
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
                .Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register(string id = "", string datetimestr = "")
        {
            // External Urls
            var datetimeparts = datetimestr.Split('_');
            DateTime? datetime = null;
            try
            {
                datetime = new DateTime(int.Parse(datetimeparts[2]),
                                        int.Parse(datetimeparts[1]),
                                        int.Parse(datetimeparts[0]));
            }
            catch (Exception ex)
            {
                id = "";
                datetime = null;
            }
            
            List<SelectListItem> countries = db.Countries
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            List<SelectListItem> states = new List<SelectListItem> {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };

            List<SelectListItem> cities = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };

            return View(new HomeRegisterViewModel
            {
            //    AvailablePackages = availablePackage
            //    VendorPackageSelected = (availablePackages
            //        .FirstOrDefault(m => m.Price == 0.0f)).Id
                Key = id,
                TimeStamp = datetime,
                Countries = countries,
                States = states,
                Cities = cities
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HomeRegisterViewModel model)
        {
            if (db.VendorDetails.FirstOrDefault(m => m.BusinessName == model.BusinessName) != null)
                ModelState.AddModelError("BusinessName", new Exception("Business name should be unique"));

            if (db.Users.FirstOrDefault(m => m.UserName == model.UserName) != null)
                ModelState.AddModelError("UserName", new Exception("Username should be unique"));

            if (db.Users.FirstOrDefault(m => m.Email == model.Email) != null)
                ModelState.AddModelError("Email", new Exception("Email address should be unique"));

            if (model.IsNewAddress)
            {
                var country = db.Countries.FirstOrDefault(m => string.Compare(m.Name.Trim(),
                    model.CountryName.Trim(), true) == 0);

                if (country != null)
                    model.SelectedCountry = country.Id;
                else
                    country = db.Countries.Add(new Country { Name = model.CountryName });

                var state = db.States.FirstOrDefault(m => string.Compare(m.Name.Trim(),
                    model.StateName.Trim(), true) == 0);

                if (state != null)
                    model.SelectedState = state.Id;
                else
                    state = db.States.Add(new State { Name = model.StateName,
                                CountryId = country.Id });

                var city = db.Places.FirstOrDefault(m => string.Compare(m.Name.Trim(),
                    model.CityName.Trim(), true) == 0);

                if (city != null)
                    model.SelectedCity = city.Id;
                else
                    city = db.Places.Add(new Place { Name = model.CityName,
                                StateId = state.Id});
            }
            
            if (ModelState.IsValid)
            {
                // model.VendorPackageSelected = int.Parse(Request.Form["PlansList"]);
                var db = new ApplicationDbContext();
                // var plan = db.VendorPackages.FirstOrDefault(m => m.Id == model.VendorPackageSelected);

                var vendor = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile,
                    //PrimaryRole = "Vendor",
                    //VendorDetails = new Utilities.VendorDetails
                    //{
                    //    BusinessName = model.BusinessName,
                    //    Zip = model.Zip,
                    //    State = model.State,
                    //    WebsiteUrl = model.WebsiteUrl,
                    //    RegistrationDate = DateTime.Now
                    //}
                };
                var result = UserManager.Create(vendor, model.Password);
                if (result.Succeeded)
                {
                    var ecomUser = db.EcomUsers.Add(new EcomUser {
                        ApplicationUserId = vendor.Id,
                        PrimaryRole = "Vendor",
                        IsActive = true,    
                        VendorDetails = new Utilities.VendorDetails
                        {
                            BusinessName = model.BusinessName,
                            WebsiteUrl = (string.IsNullOrEmpty(model.WebsiteUrl)) ? 
                                "" : "http://" + model.WebsiteUrl,
                            RegistrationDate = DateTime.Now,
                            BusinessAddress = new Address
                            {
                                Name = "Business Address",
                                ApplicationUserId = vendor.Id,
                                Line1 = model.Line1,
                                Line2 = model.Line2,
                                Line3 = model.Line3,
                                Zip = model.Zip,
                                CountryId = model.SelectedCountry,
                                StateId = model.SelectedState,
                                PlaceId = model.SelectedCity
                            }
                        }
                    });
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        if (db.Entry(ecomUser) is DbEntityEntry<EcomUser> entry)
                        {
                            entry.State = System.Data.Entity.EntityState.Deleted;
                        }
                        if (db.Entry(ecomUser.VendorDetails) is DbEntityEntry<VendorDetails> detailsEntry)
                        {
                            detailsEntry.State = System.Data.Entity.EntityState.Deleted;
                        }
                        UserManager.Delete(vendor);
                    }
                    UserManager.AddToRole(vendor.Id, "Vendor");
                    try
                    {
                        var plan = db.VendorPlans
                                    .FirstOrDefault(m => m.Price == 0.0f
                                        && m.IsActive == true
                                        && m.IsEnabled == true);
                        db.ActivePlans.Add(new Utilities.ActivePlan
                        {
                            EcomUserId = ecomUser.Id,
                            VendorPlanId = plan.Id,
                            VendorPlanPaymentDetailId = null,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddYears(1),
                            PaymentStatus = true,
                            Balance = 0.0f
                        });

                        db.SaveChanges();

                        ecomUser.VendorDetails.ActivePlanId = db.ActivePlans
                            .FirstOrDefault(m => m.EcomUserId == ecomUser.Id).Id;
                        db.Entry(ecomUser.VendorDetails).State = System.Data.Entity.EntityState.Modified;
                        db.Entry(vendor).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        SignInManager.SignIn(vendor, isPersistent: false, rememberBrowser: false);

                        if (model.TimeStamp is DateTime && 
                            db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId == model.Key) 
                                is EcomUser refferer)
                        {
                            db.Refferals.Add(new Refferal {
                                CustomerId = ecomUser.Id,
                                VendorId = refferer.Id,
                                DateOfRegistration = DateTime.Now,
                                UrlDate = model.TimeStamp
                            });
                        }

                        return RedirectToAction("Index", controllerName: "Vendor");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex);
                        db.Entry(ecomUser.VendorDetails).State = System.Data.Entity.EntityState.Deleted;
                        db.Entry(ecomUser).State = System.Data.Entity.EntityState.Deleted;
                        UserManager.Delete(vendor);
                        return View(model);
                    }
                }
                AddErrors(result);
            }

            model.Countries = db.Countries
                .OrderBy(m => m.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            model.States = new List<SelectListItem> {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };

            model.Cities = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };

            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}