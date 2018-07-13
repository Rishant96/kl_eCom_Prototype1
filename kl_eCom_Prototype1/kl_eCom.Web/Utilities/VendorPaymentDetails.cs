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
        public string KL_Notes { get; set; }

        public int VendorPackageId { get; set; }
        public VendorPackage ForPackage { get; set; }

        public String ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }
    }
}