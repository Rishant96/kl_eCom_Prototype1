using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class ActivePlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }

        [Required]
        public int VendorPlanId { get; set; }
        public VendorPlan Plan { get; set; }
        
        public bool? IsPaidFor { get; set; }

        public int? VendorPaymentDetailsId { get; set; }
        public VendorPaymentDetails PaymentDetails { get; set; }
    }
}