using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminCategoriesViewModel
    {
        public List<KL_Category> BaseCategories { get; set; }

        public Dictionary<KL_Category, List<KL_Category>> ChildCategories { get; set; }
    }

    public class AdminCategoryCreateViewModel
    {
        public Dictionary<string, int> Categories { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }

    public class AdminCategoryEditViewModel
    {
        [Required]
        public int CatId { get; set; }

        public Dictionary<string, int> Categories { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; } 
    }

    public class AdminCategoryMissingViewModel
    {
        [Required]
        public int SelectedCategoryId { get; set; }

        public Dictionary<string, int> AvailableCategories { get; set; }
        
        public List<string> MissingCategories { get; set; } 
    }

    public class AdminCategoryAllocateViewModel
    {
        [Required]
        public int SelectedCategoryId { get; set; }

        public Dictionary<string, int> AvailableCategories { get; set; }

        public List<string> VendorCategories { get; set; }
    }
}