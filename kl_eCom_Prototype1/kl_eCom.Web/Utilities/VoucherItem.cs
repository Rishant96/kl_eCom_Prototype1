using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VoucherItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; }

        [Required]
        public int Quantity { get; set; }
        
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public int? StockId { get; set; }
        public Stock StockedProduct { get; set; }
    }
}