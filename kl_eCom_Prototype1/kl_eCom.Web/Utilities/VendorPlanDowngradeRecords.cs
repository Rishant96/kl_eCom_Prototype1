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
        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }

        [Required]
        public int ActivePlanId { get; set; }
        public ActivePlan ActivePlan { get; set; }

        [Required]
        public int VendorPlanId { get; set; }
        public VendorPlan NewPlan { get; set; }
    }
}