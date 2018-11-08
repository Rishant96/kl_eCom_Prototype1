using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class OrderDeliveryDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? StartDate { get; set; }
        
        public bool IsDelivered { get; set; }

        public ICollection<PackageItem> PackageItems { get; set; }
    }

    public class PackageItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderDeliveryDetailsId { get; set; }
        public OrderDeliveryDetails DeliveryDetails { get; set; }

        [Required]
        public int StockId { get; set; }
        public Stock Stock { get; set; }
    }
}