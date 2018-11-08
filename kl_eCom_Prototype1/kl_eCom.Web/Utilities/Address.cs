using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class Address
    {
        [Key]
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
        
        public string Landmark { get; set; }

        public int? MarketId { get; set; }
        public Market Market { get; set; }

        public int? PlaceId { get; set; }
        public Place Place { get; set; }

        [Required]
        public int StateId { get; set; }
        public State State { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        [Required]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}