using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.KL_Admin.Models
{
    public class AdminDetailsViewModel 
    {
        public string Email { get; set; }
    }
    
    public class AdminEditEmailViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
    }

    public class AdminChangePasswordViewModel
    {
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string NewPassword { get; set; }
    }
}