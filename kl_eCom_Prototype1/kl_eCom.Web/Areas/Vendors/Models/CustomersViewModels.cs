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
        public List<EcomUser> Customers { get; set; }
        public Dictionary<string, bool> Registrations { get; set; }
        public Dictionary<string, bool> Buyers { get; set; }
    }

    public class CustomersOrdersViewModel
    {
        public List<int> NewOrders { get; set; }

        public List<OrderItem> ActiveOrders { get; set; }

        public List<OrderItem> PastOrders { get; set; }

        public List<OrderItem> CancellationRequested { get; set; }

        public List<OrderItem> OtherOrders { get; set; }
    }

    public class CustomersOrderDetailsViewModel
    {
        public OrderItem OrderItem { get; set; }
    
        public List<OrderStateInfo> StateInfo { get; set; }
    }
}