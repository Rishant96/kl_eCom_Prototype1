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
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public bool IsCategoryListable { get; set; }
        public bool IsActive { get; set; }
        public bool HasStock { get; set; }
        public float DefaultGST { get; set; }

        [Range(0.0, 5.0)]
        public float Rating { get; set; }

        public string ThumbnailPath { get; set; }
        public string ThumbnailMimeType { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        // public bool HasVariants { get; set; }


        [Required]
        public DateTime DateAdded { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        
        public virtual ICollection<Specification> Specifications { get; set; }

        //[NotMapped]
        //public virtual Dictionary<string, Specification> SpecificationsDict { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}