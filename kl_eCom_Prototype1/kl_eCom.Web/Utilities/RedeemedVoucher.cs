using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class RedeemedVoucher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateRedeemed { get; set; }

        [Required]
        public int ValueSaved { get; set; }

        [Required]
        public int TimesAvailed { get; set; }

        [Required]
        public int EcomUserId { get; set; }
        public EcomUser Customer { get; set; }

        [Required]
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; }
    }
}