using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Qty { get; set; }
        
        [Required]
        public int StockId { get; set; }
        public Stock Stock { get; set; }
        
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}