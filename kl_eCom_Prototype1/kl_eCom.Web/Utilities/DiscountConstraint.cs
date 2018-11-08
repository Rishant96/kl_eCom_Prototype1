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
        public int DiscountId { get; set; }
        public Discount Discount { get; set; }

        [Required]
        public DiscountConstraintType Type { get; set; }

        public ICollection<BundledItem> BundledItems { get; set; }

        public int? MinQty { get; set; }

        public float? MinOrder { get; set; }

        public int? MaxAmt { get; set; }
    }

    public class BundledItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DiscountConstraintId { get; set; }
        public DiscountConstraint DiscountConstraint { get; set; }

        [Required]
        public int StockId { get; set; }
        public Stock Stock { get; set; }
    }

    public enum DiscountConstraintType
    {
        Simple,
        MinOrder,
        Bundle,
        Qty
    }
}