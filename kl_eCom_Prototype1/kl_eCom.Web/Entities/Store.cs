using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int StoreAddressId { get; set; }
        public StoreAddress Address { get; set; }

        public ICollection<Category> Categories { get; set; }

        public string DefaultCurrencyType { get; set; }

        [Required]
        public bool IsPurchasable { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Vendor { get; set; }
    }
}