﻿using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }
    }

    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public List<string> AttributeNames { get; set; }
        public Dictionary<string, int> Attributes { get; set; }
        public Dictionary<string, string> Specifications { get; set; } 
        public bool IsActive { get; set; }
    }

    public class ProductEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime DateAdded { get; set; }
        public List<string> AttributeNames { get; set; }
        public Dictionary<string, int> Attributes { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductStockViewModel
    {
        public Product Product { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public float Price { get; set; }
        public Dictionary<string, int> Stores { get; set; }
        [Required]
        [Display(Name = "Max Per Customer")]
        public int MaxPerUser { get; set; }
        //[Required]
        public int SelectedStoreId { get; set; }
        public StockStatus Status { get; set; }
    }

    public class ProductChangeCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string SelectedCategory { get; set; }
        [Required]
        [Display(Name = "Reflect Changes in Produt Specifications")]
        public bool ReflectChange { get; set; }
    }

    public class ProductAllViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<Product, List<Stock>> Inventory { get; set; }
        public Dictionary<Product, bool> HasListing { get; set; }
    }

    public class ProductEditListedViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<Product, List<Stock>> Inventory { get; set; }
        public Dictionary<Product, bool> HasListing { get; set; }
        public Dictionary<int, bool> IsActiveList { get; set; }
        public int CurrentSelectedProducts { get; set; }
        public int MaxAllowedProducts { get; set; }
    }
}