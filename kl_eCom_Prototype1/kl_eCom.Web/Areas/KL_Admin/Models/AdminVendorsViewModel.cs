﻿using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminVendorsIndexViewModel
    {
        public List<EcomUser> Vendors { get; set; }
    }

    public class AdminVendorsDetailsViewModel
    {
        public EcomUser Vendor { get; set; }
        public VendorDetails VendorDetails { get; set; }
        public ActivePlan ActivePackage { get; set; }
        public VendorPlanDowngradeRecord DowngradeRequest { get; set; }
    }

    public class AdminVendorsEditDetailsViewModel
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BusinessName { get; set; }
    }

    public class AdminVendorsDomainEditViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime RegisterDate { get; set; }
        [Required]
        public DateTime? DomainDate { get; set; }
    }

    public class AdminVendorsPlanChangeViewModel
    {
        public string VendorName { get; set; }
        [Required]
        public string VendorId { get; set; }
        public ActivePlan ActivePlan { get; set; }
        public string Notes { get; set; }
        [Required]
        public float Amount { get; set; }
        public VendorPlanDowngradeRecord DowngradeRecord { get; set; }
    }

    public class AdminVendorsResetPasswordViewModel
    {
        [Required]
        public int VendorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class AdminVendorsSpecializationsIndexViewModel
    {
        public List<Specialization> BaseSpecializations { get; set; }
        public Dictionary<Specialization, List<Specialization>> ChildSpecializations { get; set; }
    }

    public class AdminVendorsSpecializationCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        public Dictionary<string, int> Specializations { get; set; }
        
        public int? SelectedSpecialization { get; set; }
    }

    public class AdminVendorsSpecializationEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        public Dictionary<string, int> Specializations { get; set; }

        public int? SelectedSpecialization { get; set; }
    }
}