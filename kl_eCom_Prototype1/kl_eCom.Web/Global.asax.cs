using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace kl_eCom.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string pageTitleLogo_Path = "/assets/images/KhushLIFELogo.jpg";
        public static string navbarBrandLogo_Path = "/assets/images/logo/final-logo/E-Commerce.jpg";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            var querystring = Request.QueryString;
            string source = querystring["source"];
            var db = new ApplicationDbContext();
            if (db.Users.FirstOrDefault(m => m.Id == source) 
                    is ApplicationUser vendor)
            {
                // Begin Lockout

            }
        }
    }
}
