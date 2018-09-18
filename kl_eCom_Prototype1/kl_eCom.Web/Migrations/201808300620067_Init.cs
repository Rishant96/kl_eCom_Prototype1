namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivePlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        VendorPlanId = c.Int(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        PaymentStatus = c.Boolean(nullable: false),
                        Balance = c.Single(),
                        VendorPlanPaymentDetailId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlanPaymentDetails", t => t.VendorPlanPaymentDetailId)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.VendorPlanId)
                .Index(t => t.EcomUserId)
                .Index(t => t.VendorPlanPaymentDetailId);
            
            CreateTable(
                "dbo.VendorPlanPaymentDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentType = c.Int(nullable: false),
                        AmountPaid = c.Single(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VendorPlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        DisplayName = c.String(),
                        MaxProducts = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Price = c.Single(nullable: false),
                        GST = c.Single(nullable: false),
                        ValidityPeriod = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EcomUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        PrimaryRole = c.String(nullable: false),
                        VendorDetailsId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.Refferals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        IsRegisteredUser = c.Boolean(),
                        IsBuyer = c.Boolean(),
                        UrlDate = c.DateTime(),
                        DateBuyerAdded = c.DateTime(),
                        DateOfRegistration = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EcomUsers", t => t.CustomerId)
                .ForeignKey("dbo.EcomUsers", t => t.VendorId)
                .Index(t => t.CustomerId)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StoreAddressId = c.Int(nullable: false),
                        DefaultCurrencyType = c.String(),
                        EcomUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoreAddresses", t => t.StoreAddressId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.StoreAddressId)
                .Index(t => t.EcomUserId);
            
            CreateTable(
                "dbo.StoreAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Line1 = c.String(nullable: false),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        Place = c.String(),
                        Zip = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Country = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DefaultGST = c.Single(nullable: false),
                        IsBase = c.Boolean(nullable: false),
                        ThumbnailData = c.Binary(),
                        ThumbnailMimeType = c.String(),
                        CategoryId = c.Int(),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.CategoryAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Default = c.String(),
                        InfoType = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Manufacturer = c.String(),
                        Description = c.String(),
                        IsCategoryListable = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        HasStock = c.Boolean(nullable: false),
                        DefaultGST = c.Single(nullable: false),
                        Rating = c.Single(nullable: false),
                        ThumbnailPath = c.String(),
                        ThumbnailMimeType = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagePath = c.String(),
                        ImageMimeType = c.String(),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Specifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Line1 = c.String(nullable: false),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        Zip = c.String(nullable: false),
                        State = c.String(nullable: false),
                        City = c.String(nullable: false),
                        Place = c.String(),
                        Country = c.String(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.VendorDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(nullable: false),
                        WebsiteUrl = c.String(),
                        Zip = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ActivePlanId = c.Int(),
                        RegistrationDate = c.DateTime(nullable: false),
                        DomainRegistrationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivePlans", t => t.ActivePlanId)
                .Index(t => t.ActivePlanId);
            
            CreateTable(
                "dbo.VendorPaymentGatewayDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProviderName = c.String(),
                        VendorDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId, cascadeDelete: true)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.BundledItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountConstraintId = c.Int(nullable: false),
                        StockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscountConstraints", t => t.DiscountConstraintId, cascadeDelete: true)
                .ForeignKey("dbo.Stocks", t => t.StockId, cascadeDelete: true)
                .Index(t => t.DiscountConstraintId)
                .Index(t => t.StockId);
            
            CreateTable(
                "dbo.DiscountConstraints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        MinQty = c.Int(),
                        MinOrder = c.Single(),
                        MaxAmt = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discounts", t => t.DiscountId, cascadeDelete: true)
                .Index(t => t.DiscountId);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        IsExpirable = c.Boolean(nullable: false),
                        ValidityPeriod = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsConstrained = c.Boolean(nullable: false),
                        IsPercent = c.Boolean(nullable: false),
                        Value = c.Single(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: false)
                .Index(t => t.Name, unique: true)
                .Index(t => t.EcomUserId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.DiscountedItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountId = c.Int(nullable: false),
                        StockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discounts", t => t.DiscountId, cascadeDelete: true)
                .ForeignKey("dbo.Stocks", t => t.StockId, cascadeDelete: true)
                .Index(t => t.DiscountId)
                .Index(t => t.StockId);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrentStock = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        GST = c.Single(nullable: false),
                        CurrencyType = c.String(),
                        ProductId = c.Int(nullable: false),
                        StockingDate = c.DateTime(nullable: false),
                        StoreId = c.Int(nullable: false),
                        MaxAmtPerUser = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: false)
                .Index(t => t.ProductId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Qty = c.Int(nullable: false),
                        IsEditable = c.Boolean(nullable: false),
                        StockId = c.Int(),
                        DiscountConstraintId = c.Int(),
                        CartId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.DiscountConstraints", t => t.DiscountConstraintId)
                .ForeignKey("dbo.Stocks", t => t.StockId)
                .Index(t => t.StockId)
                .Index(t => t.DiscountConstraintId)
                .Index(t => t.CartId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EcomUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.EcomUserId);
            
            CreateTable(
                "dbo.OrderStateInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderItemId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        InitialDate = c.DateTime(nullable: false),
                        FinalDate = c.DateTime(),
                        IsChangePostive = c.Boolean(),
                        RefferalId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Refferals", t => t.RefferalId, cascadeDelete: true)
                .ForeignKey("dbo.OrderItems", t => t.OrderItemId, cascadeDelete: true)
                .Index(t => t.OrderItemId)
                .Index(t => t.RefferalId);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        StockId = c.Int(),
                        DiscountConstraintId = c.Int(),
                        EcomUserId = c.Int(nullable: false),
                        Qty = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        CurrencyType = c.String(),
                        ProductName = c.String(),
                        FinalCost = c.Single(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscountConstraints", t => t.DiscountConstraintId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Stocks", t => t.StockId)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.StockId)
                .Index(t => t.DiscountConstraintId)
                .Index(t => t.EcomUserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.Int(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        AddressId = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        TotalCost = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: false)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: false)
                .Index(t => t.EcomUserId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.RedeemedVouchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateRedeemed = c.DateTime(nullable: false),
                        ValueSaved = c.Int(nullable: false),
                        TimesAvailed = c.Int(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        VoucherId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .Index(t => t.EcomUserId)
                .Index(t => t.VoucherId);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsConstrained = c.Boolean(nullable: false),
                        IsPercent = c.Boolean(nullable: false),
                        IsLimited = c.Boolean(nullable: false),
                        IsExpirable = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsAutomatic = c.Boolean(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        MaxAvailPerCustomer = c.Int(),
                        Value = c.Single(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: false)
                .Index(t => t.EcomUserId);
            
            CreateTable(
                "dbo.VoucherItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoucherId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        CategoryId = c.Int(),
                        StockId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Stocks", t => t.StockId)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .Index(t => t.VoucherId)
                .Index(t => t.CategoryId)
                .Index(t => t.StockId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.VendorPlanDowngradeRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EcomUserId = c.Int(nullable: false),
                        ActivePlanId = c.Int(nullable: false),
                        VendorPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivePlans", t => t.ActivePlanId, cascadeDelete: true)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: false)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: false)
                .Index(t => t.EcomUserId)
                .Index(t => t.ActivePlanId)
                .Index(t => t.VendorPlanId);
            
            CreateTable(
                "dbo.VendorPlanChangeRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        PlanName = c.String(),
                        Balance = c.Single(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        VendorPlanId = c.Int(nullable: false),
                        VendorPlanPaymentDetailId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlanPaymentDetails", t => t.VendorPlanPaymentDetailId)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.EcomUserId)
                .Index(t => t.VendorPlanId)
                .Index(t => t.VendorPlanPaymentDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorPlanChangeRecords", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanPaymentDetailId", "dbo.VendorPlanPaymentDetails");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RedeemedVouchers", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherItems", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.VoucherItems", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Vouchers", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.RedeemedVouchers", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.OrderStateInfoes", "OrderItemId", "dbo.OrderItems");
            DropForeignKey("dbo.OrderItems", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.OrderItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.Orders", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.OrderItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.OrderStateInfoes", "RefferalId", "dbo.Refferals");
            DropForeignKey("dbo.CartItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.CartItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.Carts", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropForeignKey("dbo.BundledItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.DiscountConstraints", "DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.Discounts", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.Discounts", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.DiscountedItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.Stocks", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Stocks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.DiscountedItems", "DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.BundledItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.ActivePlans", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.EcomUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.VendorPaymentGatewayDetails", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.VendorDetails", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.EcomUsers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stores", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.Categories", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Specifications", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.CategoryAttributes", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Stores", "StoreAddressId", "dbo.StoreAddresses");
            DropForeignKey("dbo.Refferals", "VendorId", "dbo.EcomUsers");
            DropForeignKey("dbo.Refferals", "CustomerId", "dbo.EcomUsers");
            DropForeignKey("dbo.ActivePlans", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.ActivePlans", "VendorPlanPaymentDetailId", "dbo.VendorPlanPaymentDetails");
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlanPaymentDetailId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "EcomUserId" });
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "VendorPlanId" });
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "ActivePlanId" });
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "EcomUserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.VoucherItems", new[] { "StockId" });
            DropIndex("dbo.VoucherItems", new[] { "CategoryId" });
            DropIndex("dbo.VoucherItems", new[] { "VoucherId" });
            DropIndex("dbo.Vouchers", new[] { "EcomUserId" });
            DropIndex("dbo.RedeemedVouchers", new[] { "VoucherId" });
            DropIndex("dbo.RedeemedVouchers", new[] { "EcomUserId" });
            DropIndex("dbo.Orders", new[] { "AddressId" });
            DropIndex("dbo.Orders", new[] { "EcomUserId" });
            DropIndex("dbo.OrderItems", new[] { "EcomUserId" });
            DropIndex("dbo.OrderItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.OrderItems", new[] { "StockId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderStateInfoes", new[] { "RefferalId" });
            DropIndex("dbo.OrderStateInfoes", new[] { "OrderItemId" });
            DropIndex("dbo.Carts", new[] { "EcomUserId" });
            DropIndex("dbo.CartItems", new[] { "CartId" });
            DropIndex("dbo.CartItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.CartItems", new[] { "StockId" });
            DropIndex("dbo.Stocks", new[] { "StoreId" });
            DropIndex("dbo.Stocks", new[] { "ProductId" });
            DropIndex("dbo.DiscountedItems", new[] { "StockId" });
            DropIndex("dbo.DiscountedItems", new[] { "DiscountId" });
            DropIndex("dbo.Discounts", new[] { "StoreId" });
            DropIndex("dbo.Discounts", new[] { "EcomUserId" });
            DropIndex("dbo.Discounts", new[] { "Name" });
            DropIndex("dbo.DiscountConstraints", new[] { "DiscountId" });
            DropIndex("dbo.BundledItems", new[] { "StockId" });
            DropIndex("dbo.BundledItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.VendorPaymentGatewayDetails", new[] { "VendorDetailsId" });
            DropIndex("dbo.VendorDetails", new[] { "ActivePlanId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.Addresses", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Specifications", new[] { "ProductId" });
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.CategoryAttributes", new[] { "CategoryId" });
            DropIndex("dbo.Categories", new[] { "StoreId" });
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            DropIndex("dbo.Stores", new[] { "EcomUserId" });
            DropIndex("dbo.Stores", new[] { "StoreAddressId" });
            DropIndex("dbo.Refferals", new[] { "VendorId" });
            DropIndex("dbo.Refferals", new[] { "CustomerId" });
            DropIndex("dbo.EcomUsers", new[] { "VendorDetailsId" });
            DropIndex("dbo.EcomUsers", new[] { "ApplicationUserId" });
            DropIndex("dbo.ActivePlans", new[] { "VendorPlanPaymentDetailId" });
            DropIndex("dbo.ActivePlans", new[] { "EcomUserId" });
            DropIndex("dbo.ActivePlans", new[] { "VendorPlanId" });
            DropTable("dbo.VendorPlanChangeRecords");
            DropTable("dbo.VendorPlanDowngradeRecords");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.VoucherItems");
            DropTable("dbo.Vouchers");
            DropTable("dbo.RedeemedVouchers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.OrderStateInfoes");
            DropTable("dbo.Carts");
            DropTable("dbo.CartItems");
            DropTable("dbo.Stocks");
            DropTable("dbo.DiscountedItems");
            DropTable("dbo.Discounts");
            DropTable("dbo.DiscountConstraints");
            DropTable("dbo.BundledItems");
            DropTable("dbo.VendorPaymentGatewayDetails");
            DropTable("dbo.VendorDetails");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Addresses");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Specifications");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Products");
            DropTable("dbo.CategoryAttributes");
            DropTable("dbo.Categories");
            DropTable("dbo.StoreAddresses");
            DropTable("dbo.Stores");
            DropTable("dbo.Refferals");
            DropTable("dbo.EcomUsers");
            DropTable("dbo.VendorPlans");
            DropTable("dbo.VendorPlanPaymentDetails");
            DropTable("dbo.ActivePlans");
        }
    }
}