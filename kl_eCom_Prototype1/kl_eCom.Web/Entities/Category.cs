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
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public float DefaultGST { get; set; }

        [Required]
        public bool IsBase { get; set; }

        public byte[] ThumbnailData { get; set; }
        public string ThumbnailMimeType { get; set; }
        
        [Required]
        public bool IsPurchasable { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category Parent { get; set; }

        public int? KL_CategoryId { get; set; }
        public KL_Category KL_Category { get; set; }
        
        public ICollection<CategoryAttribute> Attributes { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        [Required]
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        [NotMapped]
        public List<Category> ChildCategories { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}