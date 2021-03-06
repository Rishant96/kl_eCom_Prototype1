﻿using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public float TotalCost { get; set; }
        public Dictionary<CartItem, string> Prices { get; set; }
        public Dictionary<CartItem, string> ProductNames { get; set; }
        public Dictionary<CartItem, int> Product_MaxAllowed { get; set; }
    }

    public class CartAddViewModel
    {
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public DiscountConstraintType? Type { get; set; }
    }

    public class CartVoucherViewModel
    {
        [Required]
        public string VoucherName { get; set; } 
    }

    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string CustomerName { get; set; }
        public List<Address> Addresses { get; set; }
        public float TotalPrice { get; set; }
        public Dictionary<int, float> Prices { get; set; }
    }
}
