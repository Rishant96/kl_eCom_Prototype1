using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Models
{
    public class CustomerAddressesIndexViewModel
    {
        public List<Address> Addresses { get; set; }
    }

    public class CustomerProfileManageViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }

        public string MobileNumber { get; set; }

        public bool IsMobileVerified { get; set; }
        
        public DateTime DOB { get; set; }
    }

    public class CustomerEditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        [Required]
        public DateTime DOB { get; set; }
    }

    public class CustomerCreateAddressViewModel
    {
        [Display(Name = "Address Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }

        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        public string Landmark { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Required]
        public int SelectedCountry { get; set; }

        [Required]
        public int SelectedState { get; set; }

        public int? SelectedCity { get; set; }
    }

    public class CustomerEditAddressViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Address Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }

        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        public string Landmark { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Required]
        public int SelectedCountry { get; set; }

        [Required]
        public int SelectedState { get; set; }

        public int? SelectedCity { get; set; }
    }

    public class CustomerDeleteAddressViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Address Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }

        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        public string Landmark { get; set; }
        
        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
    }

    public class CustomerCancellationViewModel
    {
        [Required]
        public int Id { get; set; }

        public Order Order { get; set; }
    }
}