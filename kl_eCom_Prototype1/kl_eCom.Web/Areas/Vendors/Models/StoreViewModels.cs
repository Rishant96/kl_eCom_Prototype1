﻿using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
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
        public string CurrencyType { get; set; }

        [Required]
        public string Country { get; set; }
    }

    public class StoreDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public StoreAddress Address { get; set; }
        
        public string CurrencyType { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ApplicationUser Vendor { get; set; }
    }

    public class StoreEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CurrencyType { get; set; }

        [Display(Name = "Address Name")]
        public string AddrName { get; set; }

        [Required]
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }
        
        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }
    }
}