using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorPlanDowngradeRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public bool IsPending { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Vendor { get; set; }

        [Required]
        public int VendorPlanId { get; set; }
        public VendorPlan NewPlan { get; set; }
    }
}