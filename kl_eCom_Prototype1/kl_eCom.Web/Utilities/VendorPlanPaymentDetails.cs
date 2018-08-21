using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorPlanPaymentDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public PaymentType PaymentType { get; set; }
        [Required]
        public float AmountPaid { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; }
        // Other Details
    }

    public enum PaymentType
    {
        Cash,
        NetBanking,
        References,
        Other
    }
}