using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class DiscountConstraint
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DiscountConstraintType Type { get; set; }

        public int[] BundledItems { get; set; }

        public int? MinQty { get; set; }

        public float? MinOrder { get; set; }
    }

    public enum DiscountConstraintType
    {
        MinOrder,
        Bundle,
        Qty
    }
}