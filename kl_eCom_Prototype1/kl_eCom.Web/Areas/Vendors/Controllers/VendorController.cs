using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{ 
    [Authorize(Roles = "Vendor")]
    public class VendorController : Controller
    {
        // GET: Vendors/Vendor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }

        public ActionResult Edit()
        {
            // Encrypt UserId when editing
            return View();
        }
    }
}