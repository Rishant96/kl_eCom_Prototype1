using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class Refferal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int VendorId { get; set; }
        
        public EcomUser Customer { get; set; }
        public EcomUser Vendor { get; set; }

        public bool? IsRegisteredUser { get; set; }
        public bool? IsBuyer { get; set; }

        public DateTime? UrlDate { get; set; }

        public DateTime? DateBuyerAdded { get; set; }
        public DateTime? DateOfRegistration { get; set; }
    }
}