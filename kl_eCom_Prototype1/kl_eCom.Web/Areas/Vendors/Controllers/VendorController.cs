using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{ 
    [Authorize(Roles = "Vendor")]
    public class VendorController : Controller
    {
        // GET: Vendors/Vendor
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", controllerName: "Home");
            ViewBag.VendorId = id;
            return View();
        }
    }
}