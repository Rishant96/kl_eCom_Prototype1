using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class ActivePlans
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser Vendor { get; set; }
        [Required]
        public int VendorPackageId { get; set; }
        public virtual VendorPlan VendorPackage { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }        
    }
}