﻿using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderNumber { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Customer { get; set; }

        [Required]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public float TotalCost { get; set; }
    }
}