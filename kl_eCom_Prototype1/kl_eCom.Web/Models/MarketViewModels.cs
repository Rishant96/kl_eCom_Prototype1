using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class MarketIndexViewModel
    {
        public Dictionary<string, string> Vendors { get; set; }
    }

    public class MarketShopsViewModel
    {
        public List<Store> Shops { get; set; }
    }

    public class MarketSearchViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    } 
}