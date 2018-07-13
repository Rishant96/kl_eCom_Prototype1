using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminVendorsIndexViewModel
    {
        public List<ApplicationUser> Vendors { get; set; }
    }

    public class AdminVendorsDetailsViewModel
    {
        public ApplicationUser Vendor { get; set; }
        public PlanChangeRequest ChangeRequest { get; set; }
        public VendorDetails VendorDetails { get; set; }
        public ActivePackage ActivePackage { get; set; }
    }

    public class AdminVendorsDomainEditViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime RegisterDate { get; set; }
        [Required]
        public DateTime DomainDate { get; set; }
    }

    public class AdminVendorsPlanChangeViewModel
    {
        public string VendorName { get; set; }
        [Required]
        public string VendorId { get; set; }
        public string CurrentPackage { get; set; }
        [Required]
        public int NewPlanId { get; set; }
        public string NewPlanName { get; set; }
        public int NewPlanMaxProds { get; set; }
        [Required]
        public bool IsAccepted { get; set; }
        [Required]
        public bool IsPaidFor { get; set; }
        public string PaymentMode { get; set; }
        public string Notes { get; set; }
    }
}