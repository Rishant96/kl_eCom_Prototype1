using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int? StockId { get; set; }
        public Stock StockProduct { get; set; }

        public int? DiscountConstraintId { get; set; }
        public DiscountConstraint DiscountConstraint { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser Vendor { get; set; }
        
        public int Qty { get; set; }

        public float Price { get; set; }

        public string CurrencyType { get; set; }

        public string ProductName { get; set; }

        public float FinalCost { get; set; }

        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        NewOrder,
        ActiveOrder,
        Delivered,
        CancellationRequested,
        Cancelled,
        Undelivered
    }

    public enum CancellationOptions
    {
        Pending,
        Cancel,
        Reject
    }
}