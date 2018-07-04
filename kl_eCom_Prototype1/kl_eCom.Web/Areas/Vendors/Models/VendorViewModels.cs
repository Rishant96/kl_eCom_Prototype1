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
        public string UserId { get; set; }

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
        public long Mobile { get; set; }

        [Display(Name = "Website URL")]
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL")]
        public string WebsiteUrl { get; set; }

        [Required]
        [Display(Name = "Zip / Postal Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid Zip / Postal Code")]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string Confirm_Password { get; set; }

    }
}