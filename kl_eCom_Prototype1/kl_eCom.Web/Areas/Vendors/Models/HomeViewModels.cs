using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class HomeRegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Required]
        [Display(Name = "Business Owner First Name")]
        public string BusinessOwnerFirstName { get; set; }

        [Required]
        [Display(Name = "Business Owner Last Name")]
        public string BusinessOwnerLastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        public bool IsGST { get; set; }

        public string GST_Number { get; set; }
        
        [Required]
        [Display(Name = "Mobile No.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid mobile number")]
        [MaxLength(10, ErrorMessage = "Please enter a 10 digit mobile number")]
        [MinLength(10, ErrorMessage = "Please enter a 10 digit mobile number")]
        public string Mobile { get; set; }

        [Display(Name = "Website URL")]
        [RegularExpression("^[(www\\.)?a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$")]
        public string WebsiteUrl { get; set; }

        [Required]
        public string Line1 { get; set; }

        [Required]
        public string Line2 { get; set; }

        [Required]
        public string Line3 { get; set; }

        [Required]
        [Display(Name = "Zip / Postal Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid Zip / Postal Code")]
        public string Zip { get; set; }
        
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; }

        public string Key { get; set; }
        public DateTime? TimeStamp { get; set; }
     
        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Required]
        public int SelectedCountry { get; set; }

        [Required]
        public int SelectedState { get; set; }
        
        public int? SelectedCity { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }

        public string CityName { get; set; }

        // public List<VendorPackage> AvailablePackages { get; set; }
        
        // public int VendorPackageSelected { get; set; }
    }
}
