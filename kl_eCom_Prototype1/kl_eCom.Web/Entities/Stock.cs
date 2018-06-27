using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Stock")]
        public int CurrentStock { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        
        [Required]
        public DateTime StockingDate { get; set; }

        [Required]
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
    }
}