using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class CategoryAttribute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public InformationType InfoType { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public enum InformationType
    {
        Other,
        Numerical,
        String
    }
}