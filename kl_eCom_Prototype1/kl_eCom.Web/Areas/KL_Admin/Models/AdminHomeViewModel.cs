using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminHomeViewModel
    {
        public List<VendorPlan> VendorPackages { get; set; }
    }

    public class AdminFixActivesViewModel
    {
        public List<string> Vendors { get; set; }
        public Dictionary<string, List<string>> ProductsDeactivated { get; set; }
    }
}