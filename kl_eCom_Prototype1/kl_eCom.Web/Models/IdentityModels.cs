using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;
using kl_eCom.Web.Entities;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace kl_eCom.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public Sex Sex { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public enum Sex
    {
        Male,
        Female,
        Other
    }

    public class EcomUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string PrimaryRole { get; set; }

        public int? VendorDetailsId { get; set; }
        public VendorDetails VendorDetails { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Refferal> AssociatedCustomerOf { get; set; }
        public virtual ICollection<Refferal> AssociatedVendorOf { get; set; }
        
        public virtual ICollection<Store> Stores { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EcomUser>()
                .HasMany(u => u.AssociatedCustomerOf) // <--
                .WithRequired(r => r.Customer) // <--
                .HasForeignKey(r => r.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EcomUser>()
                .HasMany(u => u.AssociatedVendorOf) // <--
                .WithRequired(r => r.Vendor) // <--
                .HasForeignKey(r => r.VendorId)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Discount>()
                .Property(t => t.Name)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_Name") { IsUnique = true }));
        }

        public DbSet<EcomUser> EcomUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CategoryAttribute> Attributes { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<VendorPlan> VendorPlans { get; set; }
        public DbSet<ActivePlan> ActivePlans { get; set; }
        public DbSet<VendorPlanChangeRecord> VendorPlanChangeRecord { get; set; }
        public DbSet<VendorDetails> VendorDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Refferal> Refferals { get; set; }
        public DbSet<OrderStateInfo> OrderInformation { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountedItem> DiscountedItems { get; set; }
        public DbSet<DiscountConstraint> DiscountConstraints { get; set; }
        public DbSet<BundledItem> BundledItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherItem> VoucherItems { get; set; }
        public DbSet<RedeemedVoucher> RedeemedVouchers { get; set; }
        public DbSet<VendorPlanDowngradeRecord> VendorDowngradeRecords { get; set; }
        public DbSet<KL_Category> KL_Categories { get; set; } 
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<VendorSpecialization> VendorSpecializations { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}