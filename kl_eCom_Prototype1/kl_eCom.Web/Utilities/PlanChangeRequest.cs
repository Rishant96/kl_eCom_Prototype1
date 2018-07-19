using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class PlanChangeRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }

        [Required]
        public int VendorPlanId { get; set; }
        public VendorPlan RequestedPackage { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        public DateTime? DecisionDate { get; set; }
    }

    public enum RequestStatus
    {
        Pending,
        Accepted,
        Dismissed
    }
}