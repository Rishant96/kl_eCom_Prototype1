using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kl_eCom.Web.Areas.Vendors.Models
{
    public class VendorEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Mobile No.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid mobile number")]
        [MaxLength(10, ErrorMessage = "Please enter a 10 digit mobile number")]
        public string Mobile { get; set; }
        [Display(Name = "Website URL")]
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL")]
        public string WebsiteUrl { get; set; }
        [Required]
        [Display(Name = "Zip / Postal Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid Zip / Postal Code")]
        public string Zip { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
    }

    public class VendorSpecialityViewModel
    {
        public int[] SelectedSpecialities { get; set; }
        public Dictionary<string, int> AllSpecialities { get; set; }
        public Dictionary<string, List<string>> BaseSpecialityDict { get; set; }
    }

    public class VendorPasswordChangeViewModel
    {
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string NewPassword { get; set; }
    }

    public class VendorPlanIndexViewModel
    {
        public string UserName { get; set; }
        public VendorPlan CurrentPackage { get; set; }
        public VendorPlanPaymentDetail PaymentDetails { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? DaysLeft { get; set; }
        public float Balance { get; set; }
    }

    public class VendorPlanChangeViewModel
    {
        public string UserName { get; set; }
        public int CurrentPlan { get; set; }
        public List<VendorPlan> AvailablePlans { get; set; }
        [Required]
        [Range((int)1, int.MaxValue, ErrorMessage = "Please select a valid plan")]
        public int Selection { get; set; }
    }

    public class VendorPlanConfirmViewModel
    {
        public string UserName { get; set; }
        public VendorPlan CurrentPlan { get; set; }
        public VendorPlan NewPlan { get; set; }
        public float Difference { get; set; }
        public bool IsUpgrade { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class VendorDiscountsViewModel
    {
        public List<int> Ids { get; set; }
        public Dictionary<int, string> Names { get; set; }
        public Dictionary<int, string> Statuses { get; set; }
        public Dictionary<int, int> DiscountIds { get; set; }
        public Dictionary<int, string> StartDates { get; set; }
        public Dictionary<int, string> ValidityPeriods { get; set; }
        public Dictionary<int, string> DiscountValues { get; set; }
        public Dictionary<int, string> StoreNames { get; set; }
        public Dictionary<int, List<string>> DiscountTypes { get; set; }
        public Dictionary<int, List<string>> Products { get; set; }
    }

    public class VendorDiscountCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Is Expirable?")]
        public bool IsExpirable { get; set; }
        [Display(Name = "Validity Period")]
        public int? ValidityPeriod { get; set; }
        [Required]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please provide a value")]
        public float Value { get; set; }
        public List<Category> AvailableCategories { get; set; }
        public Dictionary<Category, List<Product>> AvailableProducts { get; set; }
        public string ExtraInfo { get; set; }
    }

    public class VendorDiscountDetailsViewModel
    {
        public Discount Discount { get; set; }
        public DiscountConstraint Constraint { get; set; }
        public List<Product> Products { get; set; }
    }

    public class VendorDiscountEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Is Expirable?")]
        public bool IsExpirable { get; set; }
        [Display(Name = "Validity Period")]
        public int? ValidityPeriod { get; set; }
        [Required]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please provide a value")]
        public float Value { get; set; }
        public List<Category> AvailableCategories { get; set; }
        public Dictionary<Category, List<Product>> AvailableProducts { get; set; }
        public string Type { get; set; }
        public string ExtraInfo { get; set; }
        public int MaxAmt { get; set; }
        [Required]
        public DiscountConstraintType DiscountType { get; set; }
        public int[] SelectedProducts { get; set; }
    }

    public class VendorVouchersIndexViewModel
    {
        public List<Voucher> Vouchers { get; set; }
        public List<RedeemedVoucher> RedeemedVouchers { get; set; }
        public string DefaultCurrencyType { get; set; }
    }

    public class VendorVoucherCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Has Constraints?")]
        public bool IsConstrained { get; set; }
        [Required]
        [Display(Name = "Limited Use")]
        public bool IsLimited { get; set; }
        [Required]
        [Display(Name = "Does Expire?")]
        public bool IsExpirable { get; set; }
        [Required]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "per Customer Limit")]
        public int? MaxAvailPerCustomer { get; set; }
        [Required]
        public float Value { get; set; }
    }

    public class VendorVoucherItemPartialModel
    {
        private static int _counter = 0;
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool IsProductSpecific { get; set; }
        [Required]
        public int Count { get; set; }
        public List<Category> AvailableCategories { get; set; }
        public Dictionary<Category, List<Product>> AvailableProducts { get; set; }
        public VendorVoucherItemPartialModel()
        {
            Count = _counter++;
        }
    }

    public class VendorVoucherEditViewModel
    {

    }
}
