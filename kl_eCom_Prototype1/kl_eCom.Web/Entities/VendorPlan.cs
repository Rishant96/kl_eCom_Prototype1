using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class VendorPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public int MaxProducts { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        [Range(minimum: 0.0, maximum: 100.0, ErrorMessage = "Please enter a value between 0.0 to 100.0")]
        public float GST { get; set; }
        public int? ValidityPeriod { get; set; }

        public virtual ICollection<VendorPlanChangeRecord> NewPlanChanges { get; set; }
        public virtual ICollection<VendorPlanChangeRecord> OldPlanChanges { get; set; }
    }
}