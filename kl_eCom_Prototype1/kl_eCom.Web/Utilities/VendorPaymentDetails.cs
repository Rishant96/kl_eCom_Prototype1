using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorPaymentDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PaymentMode { get; set; }
        public string Details { get; set; }

        public int VendorPlanId { get; set; }
        public VendorPlan ForPackage { get; set; }

        public String ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }
    }
}