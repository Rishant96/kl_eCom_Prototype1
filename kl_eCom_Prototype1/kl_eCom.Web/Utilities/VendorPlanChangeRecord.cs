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

        public DateTime StartDate { get; set; }
        public DateTime TimeStamp { get; set; }

        public string PlanName { get; set; }

        public float Balance { get; set; }

        public int EcomUserId { get; set; }
        public EcomUser Vendor { get; set; }

        public int VendorPlanId { get; set; }
        public VendorPlan Plan { get; set; }

        public int? VendorPlanPaymentDetailId { get; set; }
        public VendorPlanPaymentDetail PaymentDetail { get; set; }
    }
}