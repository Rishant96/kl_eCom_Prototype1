using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using kl_eCom.Web.Utilities;

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
        public float GST { get; set; }

        public string CurrencyType { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
               
        [Required]
        public DateTime StockingDate { get; set; }

        [Required]
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        public int MaxAmtPerUser { get; set; }

        public StockStatus Status { get; set; }
        
        public float GetPrice()
        {
            float price = this.Price;

            ApplicationDbContext db = new ApplicationDbContext();

            var discountIds = db.DiscountedItems
                                .Include(m => m.Discount)
                                .Where(m => m.StockId == this.Id
                                    && m.Discount.IsActive
                                    && DateTime.Compare(m.Discount.StartDate, DateTime.Now) < 0
                                   && (!m.Discount.IsExpirable
                                       || DateTime.Compare(m.Discount.EndDate ??
                                       m.Discount.StartDate, DateTime.Now) > 0)
                                   )
                                .Select(m => m.DiscountId)
                                .ToList();

            var discountConstraints = db.DiscountConstraints
                                        .Include(m => m.Discount)
                                        .Include(m => m.Discount.DiscountedItems)
                                        .Include(m => m.Discount.Store)
                                        .Where(m => discountIds
                                            .Contains(m.DiscountId)
                                            && m.Discount.IsActive)
                                        .ToList();

            foreach (var constraint in discountConstraints)
            {
                if (constraint.Type == DiscountConstraintType.Simple)
                {
                    if (constraint.Discount.IsPercent)
                    {
                        price = this.Price *
                                    ((100 - constraint.Discount.Value) / 100);
                    }
                    else
                    {
                        price = this.Price -
                                    constraint.Discount.Value;
                    }
                }
            }

            return price;
        }
    }

    public enum StockStatus
    {
        OutOfStock,
        InStock,
    }
}