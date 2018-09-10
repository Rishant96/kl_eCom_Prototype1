using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Models
{
    public class KL_Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int? KL_CategoryId { get; set; }
        public virtual KL_Category ParentCategory { get; set; }  

        public ICollection<KL_Category> ChildCategories { get; set; }
    }
}