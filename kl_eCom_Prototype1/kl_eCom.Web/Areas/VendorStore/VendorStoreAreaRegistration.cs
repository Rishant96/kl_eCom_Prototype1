using System.Web.Mvc;

namespace kl_eCom.Web.Areas.VendorStore
{
    public class VendorStoreAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VendorStore";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VendorStore_default",
                "VendorStore/{vendorId}/{controller}/{action}/{id}",
                new { vendorId = 0, controller = "Products", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}