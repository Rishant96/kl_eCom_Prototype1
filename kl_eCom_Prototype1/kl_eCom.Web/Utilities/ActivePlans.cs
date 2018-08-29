using kl_eCom.Web.Entities;
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
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public int VendorPlanId { get; set; }
        public VendorPlan Plan { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Vendor { get; set; }
        
        public bool PaymentStatus { get; set; }
        public float? Balance { get; set; }

        public int? VendorPlanPaymentDetailId { get; set; }
        public VendorPlanPaymentDetail PaymentDetail { get; set; }
    }
}