using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser Owner { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }
}