using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class ShopIndexViewModel
    {
        public List<Category> Categories { get; set; }
        public Dictionary<Category, bool> IsLeafDict { get; set; }
    }

    public class ShopProductsViewModel
    {
        public List<Stock> Stocks { get; set; }
        public int Qty { get; set; }
    }
}