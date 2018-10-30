using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Utilities
{
    public class VendorDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BusinessName { get; set; }
        [Display(Name = "Website URL")]
        public string WebsiteUrl { get; set; }
        public int? ActivePlanId { get; set; }
        [Display(Name = "Active Plan")]
        public ActivePlan ActivePlan { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        public DateTime? DomainRegistrationDate { get; set; }

        public byte[] Logo_Img_Data { get; set; }
        public string Logo_Mime_Type { get; set; }

        [Required]
        public int AddressId { get; set; }
        public Address BusinessAddress { get; set; }

        public ICollection<VendorPaymentGatewayDetail> PaymentGatewayDetails { get; set; } 
        public ICollection<VendorSpecialization> Specializations { get; set; }
    }

    public class VendorPaymentGatewayDetail
    {
        [Key]
        public int Id { get; set; }

        public PaymentGatewayProvider ProviderName { get; set; }

        [Required]
        public int VendorDetailsId { get; set; }
        public VendorDetails VendorDetails { get; set; }

        // Gateway Information 
    }

    public enum PaymentGatewayProvider
    {
        Paytm 
    }
}