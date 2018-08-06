using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsExpirable { get; set; }
        
        public int? ValidityPeriod { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsConstrained { get; set; }

        [Required]
        public bool IsPercent { get; set; }

        [Required]
        public float Value { get; set; }

        [Required]
        public int StoreId { get; set; }
        public Store Store { get; set; }
        
        public ICollection<DiscountedItem> DiscountedItems { get; set; }
    }
}