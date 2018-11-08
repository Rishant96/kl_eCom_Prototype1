using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminPlansIndexViewModel
    {
        public List<VendorPlan> VendorPackages { get; set; }
    }

    public class AdminPlansCreateViewModel
    {
        [Required]
        public bool IsActive { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public float Price { get; set; }
        [Required]
        [Range(minimum: 0.0, maximum: 100.0, ErrorMessage = "Please enter a value between 0.0 to 100.0")]
        public float GST { get; set; }
        [Required]
        public bool IsExpirable { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        public int? ValidityPeriod { get; set; }
        [Required]
        public int MaxProducts { get; set; }
    }

    public class AdminPlansEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public float Price { get; set; }
        [Required]
        [Range(minimum: 0.0, maximum: 100.0, ErrorMessage = "Please enter a value between 0.0 to 100.0")]
        public float GST { get; set; }
        [Required]
        public bool IsExpirable { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        public int? ValidityPeriod { get; set; }
        [Required]
        public int MaxProducts { get; set; }
    }

    public class AdminPlansDetailsViewModel
    {
        public VendorPlan VendorPackage { get; set; }
    }
}