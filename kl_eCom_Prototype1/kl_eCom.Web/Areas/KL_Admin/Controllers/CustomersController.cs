using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.KL_Admin.Controllers
{
    public class CustomersController : Controller
    {
        // GET: KL_Admin/Customers
        public ActionResult Index()
        {
            return View();
        }
    }
}