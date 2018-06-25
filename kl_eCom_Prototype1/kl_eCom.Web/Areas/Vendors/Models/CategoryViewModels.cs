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

    public class AddAttributeViewModel
    {
        [Required]
        [Display(Name = "Attribute Name")]
        public string AtrbName { get; set; }
    }
}