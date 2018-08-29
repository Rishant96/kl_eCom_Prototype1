using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        [RegularExpression("^[(www\\.)?a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$")]
        public string WebsiteUrl { get; set; }

        [Required]
        [Display(Name = "Zip / Postal Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid Zip / Postal Code")]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; }

        public string Key { get; set; }
        public DateTime? TimeStamp { get; set; }
        
        //public List<VendorPackage> AvailablePackages { get; set; }
        
        //public int VendorPackageSelected { get; set; }
    }
}