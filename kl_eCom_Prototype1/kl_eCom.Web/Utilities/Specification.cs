using kl_eCom.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class Specification
    {
        [Key]
        public int Id { get; set; }
        // public bool IsVariable { get; set; }

        // public virtual ICollection<SpecOption> SpecOptions { get; set; }
        public string Value { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}