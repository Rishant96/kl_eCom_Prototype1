using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminCustomersIndexViewModel
    {
        public List<EcomUser> Customers { get; set; }
    }

    public class AdminCustomersDetailsViewModel
    {
        public EcomUser EcomUser { get; set; }
    }

    public class AdminCustomersResetPasswordViewModel
    {
        [Required]
        public int CustomerId { get; set; }
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
}