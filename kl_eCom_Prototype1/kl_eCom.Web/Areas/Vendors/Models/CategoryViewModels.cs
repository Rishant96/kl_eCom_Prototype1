using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
    }

    public class AddAttributeViewModel
    {
        private static int _counter = 0;

        [Required]
        [Display(Name = "Attribute Name")]
        public string AtrbName { get; set; }

        [Required]
        public int Count { get; private set; }

        public AddAttributeViewModel()
        {
            Count = _counter++;
        }
    }
}