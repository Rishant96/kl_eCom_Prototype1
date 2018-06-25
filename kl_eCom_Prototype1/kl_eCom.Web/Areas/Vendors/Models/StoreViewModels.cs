using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class StoreIndexViewModel
    {
        public List<Store> Stores { get; set; }
        public Dictionary<Store, List<Stock>> Stocks { get; set; }
    }

    public class StoreCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }

        [Required]
        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Required]
        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }
    }
}