using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class CustomersIndexViewModel
    {
        public List<ApplicationUser> Customers { get; set; }
        public Dictionary<string, bool> Registrations { get; set; }
        public Dictionary<string, bool> Buyers { get; set; }
    }

    public class CustomersOrdersViewModel
    {
        public List<Order> ActiveOrders { get; set; }
        public Dictionary<int, List<OrderItem>> ActiveOrderItems { get; set; }

        public List<Order> PastOrders { get; set; }
        public Dictionary<int, List<OrderItem>> PastOrderItems { get; set; }

        public List<Order> CancellationRequested { get; set; }
        public Dictionary<int, List<OrderItem>> CancellationOrderItems { get; set; }

        public List<Order> OtherOrders { get; set; }
        public Dictionary<int, List<OrderItem>> OtherOrderItems { get; set; }
    }
}