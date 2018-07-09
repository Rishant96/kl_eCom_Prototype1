using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [DataType(DataType.Url)]
        public string WebsiteUrl { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public VendorPackage Package { get; set; }
        
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}