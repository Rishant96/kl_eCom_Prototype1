﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorPackage
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
        public int? ValidityPeriod { get; set; }
    }
}