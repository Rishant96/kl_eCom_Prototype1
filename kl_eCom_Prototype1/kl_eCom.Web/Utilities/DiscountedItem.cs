using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class DiscountedItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DiscountId { get; set; }
        public Discount Discount { get; set; }

        [Required]
        public int StockId { get; set; }
        public Stock StockedProduct { get; set; }
    }
}