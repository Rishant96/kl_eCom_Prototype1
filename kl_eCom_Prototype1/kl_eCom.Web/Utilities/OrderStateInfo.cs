using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class OrderStateInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }

        [Required]
        public ChangeType Type { get; set; }

        [Required]
        public DateTime InitialDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public bool? IsChangePostive { get; set; }

        [Required]
        public int RefferalId { get; set; }
        public Refferal Actors { get; set; }
    }

    public enum ChangeType
    {
        Activated,
        Cancellation,
        Delivered,
        DeliveryFailed
    }
}