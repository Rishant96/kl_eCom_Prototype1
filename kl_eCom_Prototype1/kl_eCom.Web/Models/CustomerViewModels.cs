using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class CustomerAddressesIndexViewModel
    {
        public List<Address> Addresses { get; set; }
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

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Display(Name = "Landmark")]
        public string Place { get; set; }

        [Required]
        public string Country { get; set; }
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

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }


        [Display(Name = "Landmark")]
        public string Place { get; set; }

        [Required]
        public string Country { get; set; }
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

        public string State { get; set; }

        public string City { get; set; }

        [Display(Name = "Landmark")]
        public string Place { get; set; }

        public string Country { get; set; }
    }

    public class CustomerCancellationViewModel
    {
        [Required]
        public int Id { get; set; }

        public Order Order { get; set; }
    }
}