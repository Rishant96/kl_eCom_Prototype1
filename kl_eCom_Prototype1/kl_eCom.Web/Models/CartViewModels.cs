using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public float TotalCost { get; set; }
        public Dictionary<CartItem, float> Prices { get; set; }
        public Dictionary<CartItem, string> ProductNames { get; set; }
    }

    public class CartAddViewModel
    {
        public int StockId { get; set; }
        public int Qty { get; set; }
    }
}