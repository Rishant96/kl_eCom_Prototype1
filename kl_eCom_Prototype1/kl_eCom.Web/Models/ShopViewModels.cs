using kl_eCom.Web.Entities;
using kl_eCom.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class ShopIndexViewModel
    {
        public Dictionary<string, int> Breadcrum { get; set; } 
        public List<Category> Categories { get; set; }
        public Dictionary<Category, bool> IsLeafDict { get; set; }
    }

    public class ShopProductsViewModel
    {
        public Dictionary<string, int> Breadcrum { get; set; }
        public List<Stock> Stocks { get; set; }
        public int Qty { get; set; }
        public Dictionary<int, int> Max { get; set; }
        [Required]
        public SortOption SelectedOption { get; set; }
    }
}