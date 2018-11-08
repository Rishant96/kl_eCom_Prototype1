using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Entities
{
    public class Specialization
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }

        public bool IsVisible { get; set; }

        public int? SpecializationId { get; set; }
        public Specialization ParentSpecialization { get; set; }

        public ICollection<VendorSpecialization> AssociatedVendors { get; set; }
    }

    public class VendorSpecialization
    {
        [Key]
        public int Id { get; set; }

        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }

        public int VendorDetailsId { get; set; }
        public VendorDetails VendorDetails { get; set; }
    }
}