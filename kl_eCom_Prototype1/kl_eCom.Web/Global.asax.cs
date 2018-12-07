using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        public static int? vendorId = null;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-Frame-Options");
            Response.AddHeader("X-Frame-Options", "AllowAll");
        }

        protected void Application_BeginRequest()
        {
            if (Request.Url.Segments.Count() > 1
                    && Request.Url.Segments[1] == "VendorStore/")
            {
                //var ApplicationUserManager = this.Context.GetOwinContext().Authentication;
                //ApplicationUserManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            }
        }
    }
}
