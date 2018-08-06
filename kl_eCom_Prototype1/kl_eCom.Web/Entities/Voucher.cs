using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public bool IsConstrained { get; set; }

        [Required]
        public bool IsPercent { get; set; }

        [Required]
        public bool IsLimited { get; set; }

        [Required]
        public bool IsExpirable { get; set; }

        [Required]
        public bool IsActive { get; set; }
        
        public bool? IsAutomatic { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public int? MaxAvailPerCustomer { get; set; }

        [Required]
        public float Value { get; set; }
        
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }

        public ICollection<VoucherItem> VoucherItems { get; set; }
    }
}