﻿using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class VendorEditViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile No.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid mobile number")]
        [MaxLength(10, ErrorMessage = "Please enter a 10 digit mobile number")]
        [MinLength(10, ErrorMessage = "Please enter a 10 digit mobile number")]
        public string Mobile { get; set; }

        [Display(Name = "Website URL")]
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL")]
        public string WebsiteUrl { get; set; }

        [Required]
        [Display(Name = "Zip / Postal Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid Zip / Postal Code")]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        //[Required]
        //[DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        //public string OldPassword { get; set; }

        //[Required]
        //[DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        //public string NewPassword { get; set; }

        //[Required]
        //[Display(Name = "Confirm Password")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        //public string Confirm_Password { get; set; }

    }

    public class VendorPasswordChangeViewModel
    {
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string Confirm_Password { get; set; }
    }

    public class VendorPlanIndexViewModel
    {
        public string UserName { get; set; }
        public VendorPlan CurrentPackage { get; set; }
        public VendorPaymentDetails PaymentDetails { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class VendorPlanChangeViewModel
    {
        public string UserName { get; set; }
        public VendorPlan CurrentPackage { get; set; }
        public List<string> Packages { get; set; }
        [Required]
        public string SelectedPackage { get; set; }
    }

    public class VendorDiscountsViewModel
    {
        public List<int> Ids { get; set; }
        public Dictionary<int, string> Names { get; set; }
        public Dictionary<int, int> DiscountIds { get; set; }
        public Dictionary<int, string> StartDates { get; set; }
        public Dictionary<int, string> ValidityPeriods { get; set; }
        public Dictionary<int, string> DiscountValues { get; set; }
        public Dictionary<int, string> StoreNames { get; set; }
        public Dictionary<int, string> DiscountTypes { get; set; }
    }

    public class VendorDiscountCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "Is Expirable?")]
        public bool IsExpirable { get; set; }

        [Required]
        [Display(Name = "Validity Period")]
        public int? ValidityPeriod { get; set; }

        [Required]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Advanced Discount")]
        public bool IsConstrained { get; set; }

        [Required]
        public float Value { get; set; }
        
        public List<Category> AvailableCategories { get; set; }

        public Dictionary<Category, List<Product>> AvailableProducts { get; set; }
    }
}
