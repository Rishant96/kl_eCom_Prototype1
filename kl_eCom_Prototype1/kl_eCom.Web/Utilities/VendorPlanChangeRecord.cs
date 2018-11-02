using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorPlanChangeRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OldStartDate { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
        
        [Required]
        public string OldPlanName { get; set; }

        [Required]
        public string NewPlanName { get; set; }

        [Required]
        public float OldBalance { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Vendor { get; set; }
        
        public int? OldVendorPlanId { get; set; }
        public VendorPlan OldPlan { get; set; }
        
        public int? NewVendorPlanId { get; set; }
        public VendorPlan NewPlan { get; set; }
        
        public int? VendorPlanPaymentDetailId { get; set; }
        public VendorPlanPaymentDetail PaymentDetail { get; set; }
    }
}