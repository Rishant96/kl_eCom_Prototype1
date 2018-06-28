using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class CategoryCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CategoryAttribute> Attributes { get; set; }
    }

    public class CategoryEditViewModel
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CategoryAttribute> Attributes { get; set; }
        [Display(Name = "Reflect Changes in Products")]
        [Required]
        public bool ReflectChange { get; set; }
    }
    
    public class CategoryImportProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Reflect Change in product Specifications")]
        public bool ReflectChange { get { return true; } }
    }

    public class AddAttributeViewModel
    {
        private static int _counter = 0;

        [Required]
        [Display(Name = "Attribute Name")]
        public string AtrbName { get; set; }
        
        public string Default { get; set; }

        [Required]
        public InformationType Type { get; set; }

        [Required]
        public int Count { get; private set; }

        public AddAttributeViewModel()
        {
            Count = _counter++;
        }
    }
}