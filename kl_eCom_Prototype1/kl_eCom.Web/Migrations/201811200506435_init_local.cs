namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init_local : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivePlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                        PaymentDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                "dbo.VendorPlanChangeRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldStartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        TimeStamp = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OldPlanName = c.String(nullable: false),
                        NewPlanName = c.String(nullable: false),
                        OldBalance = c.Single(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        NewVendorPlanId = c.Int(),
                        VendorPlanPaymentDetailId = c.Int(),
                        VendorPlan_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlans", t => t.NewVendorPlanId)
                .ForeignKey("dbo.VendorPlanPaymentDetails", t => t.VendorPlanPaymentDetailId)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlan_Id)
                .Index(t => t.EcomUserId)
                .Index(t => t.NewVendorPlanId)
                .Index(t => t.VendorPlanPaymentDetailId)
                .Index(t => t.VendorPlan_Id);
            
            CreateTable(
                "dbo.EcomUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        PrimaryRole = c.String(nullable: false),
                        VendorDetailsId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Line1 = c.String(nullable: false),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        Landmark = c.String(),
                        MarketId = c.Int(),
                        PlaceId = c.Int(),
                        StateId = c.Int(nullable: false),
                        Zip = c.String(nullable: false),
                        CountryId = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        EcomUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.Places", t => t.PlaceId)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUser_Id)
                .Index(t => t.MarketId)
                .Index(t => t.PlaceId)
                .Index(t => t.StateId)
                .Index(t => t.CountryId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.EcomUser_Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: false)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.Markets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        PlaceId = c.Int(nullable: false),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Places", t => t.PlaceId, cascadeDelete: true)
                .Index(t => t.PlaceId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DOB = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Sex = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastLogin = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
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
                "dbo.Refferals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        IsRegisteredUser = c.Boolean(),
                        IsBuyer = c.Boolean(),
                        UrlDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        DateBuyerAdded = c.DateTime(precision: 7, storeType: "datetime2"),
                        DateOfRegistration = c.DateTime(precision: 7, storeType: "datetime2"),
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
                        Landmark = c.String(),
                        MarketId = c.Int(),
                        PlaceId = c.Int(),
                        Zip = c.String(nullable: false),
                        StateId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.Places", t => t.PlaceId)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .Index(t => t.MarketId)
                .Index(t => t.PlaceId)
                .Index(t => t.StateId)
                .Index(t => t.CountryId);
            
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
                        KL_CategoryId = c.Int(),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KL_Category", t => t.KL_CategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.KL_CategoryId)
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
                "dbo.KL_Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        ImagePath = c.String(),
                        KL_CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KL_Category", t => t.KL_CategoryId)
                .Index(t => t.KL_CategoryId);
            
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
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                "dbo.VendorDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(nullable: false),
                        WebsiteUrl = c.String(),
                        ActivePlanId = c.Int(),
                        RegistrationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DomainRegistrationDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Logo_Img_Data = c.Binary(),
                        Logo_Mime_Type = c.String(),
                        AddressId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivePlans", t => t.ActivePlanId)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .Index(t => t.ActivePlanId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.VendorPaymentGatewayDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProviderName = c.Int(nullable: false),
                        VendorDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId, cascadeDelete: true)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.VendorSpecializations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SpecializationId = c.Int(nullable: false),
                        VendorDetailsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specializations", t => t.SpecializationId, cascadeDelete: true)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId, cascadeDelete: true)
                .Index(t => t.SpecializationId)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.Specializations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsVisible = c.Boolean(nullable: false),
                        SpecializationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specializations", t => t.SpecializationId)
                .Index(t => t.SpecializationId);
            
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
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(precision: 7, storeType: "datetime2"),
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
                        StockingDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                        InitialDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        FinalDate = c.DateTime(precision: 7, storeType: "datetime2"),
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
                        OrderDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                        DateRedeemed = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(precision: 7, storeType: "datetime2"),
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
                        IsPending = c.Boolean(nullable: false),
                        EcomUserId = c.Int(nullable: false),
                        VendorPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.EcomUsers", t => t.EcomUserId, cascadeDelete: true)
                .Index(t => t.EcomUserId)
                .Index(t => t.VendorPlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "VendorPlanId", "dbo.VendorPlans");
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
            DropForeignKey("dbo.ActivePlans", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlan_Id", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.EcomUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.VendorSpecializations", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.Specializations", "SpecializationId", "dbo.Specializations");
            DropForeignKey("dbo.VendorSpecializations", "SpecializationId", "dbo.Specializations");
            DropForeignKey("dbo.VendorPaymentGatewayDetails", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.VendorDetails", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.VendorDetails", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.EcomUsers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stores", "EcomUserId", "dbo.EcomUsers");
            DropForeignKey("dbo.Categories", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Specifications", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "KL_CategoryId", "dbo.KL_Category");
            DropForeignKey("dbo.KL_Category", "KL_CategoryId", "dbo.KL_Category");
            DropForeignKey("dbo.CategoryAttributes", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Stores", "StoreAddressId", "dbo.StoreAddresses");
            DropForeignKey("dbo.StoreAddresses", "StateId", "dbo.States");
            DropForeignKey("dbo.StoreAddresses", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.StoreAddresses", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.StoreAddresses", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Refferals", "VendorId", "dbo.EcomUsers");
            DropForeignKey("dbo.Refferals", "CustomerId", "dbo.EcomUsers");
            DropForeignKey("dbo.Addresses", "EcomUser_Id", "dbo.EcomUsers");
            DropForeignKey("dbo.Addresses", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "StateId", "dbo.States");
            DropForeignKey("dbo.Addresses", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.Addresses", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Places", "StateId", "dbo.States");
            DropForeignKey("dbo.Markets", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.States", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanPaymentDetailId", "dbo.VendorPlanPaymentDetails");
            DropForeignKey("dbo.VendorPlanChangeRecords", "NewVendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.ActivePlans", "VendorPlanPaymentDetailId", "dbo.VendorPlanPaymentDetails");
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "VendorPlanId" });
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
            DropIndex("dbo.Specializations", new[] { "SpecializationId" });
            DropIndex("dbo.VendorSpecializations", new[] { "VendorDetailsId" });
            DropIndex("dbo.VendorSpecializations", new[] { "SpecializationId" });
            DropIndex("dbo.VendorPaymentGatewayDetails", new[] { "VendorDetailsId" });
            DropIndex("dbo.VendorDetails", new[] { "AddressId" });
            DropIndex("dbo.VendorDetails", new[] { "ActivePlanId" });
            DropIndex("dbo.Specifications", new[] { "ProductId" });
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.KL_Category", new[] { "KL_CategoryId" });
            DropIndex("dbo.CategoryAttributes", new[] { "CategoryId" });
            DropIndex("dbo.Categories", new[] { "StoreId" });
            DropIndex("dbo.Categories", new[] { "KL_CategoryId" });
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            DropIndex("dbo.StoreAddresses", new[] { "CountryId" });
            DropIndex("dbo.StoreAddresses", new[] { "StateId" });
            DropIndex("dbo.StoreAddresses", new[] { "PlaceId" });
            DropIndex("dbo.StoreAddresses", new[] { "MarketId" });
            DropIndex("dbo.Stores", new[] { "EcomUserId" });
            DropIndex("dbo.Stores", new[] { "StoreAddressId" });
            DropIndex("dbo.Refferals", new[] { "VendorId" });
            DropIndex("dbo.Refferals", new[] { "CustomerId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Markets", new[] { "PlaceId" });
            DropIndex("dbo.Places", new[] { "StateId" });
            DropIndex("dbo.States", new[] { "CountryId" });
            DropIndex("dbo.Addresses", new[] { "EcomUser_Id" });
            DropIndex("dbo.Addresses", new[] { "ApplicationUserId" });
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropIndex("dbo.Addresses", new[] { "StateId" });
            DropIndex("dbo.Addresses", new[] { "PlaceId" });
            DropIndex("dbo.Addresses", new[] { "MarketId" });
            DropIndex("dbo.EcomUsers", new[] { "VendorDetailsId" });
            DropIndex("dbo.EcomUsers", new[] { "ApplicationUserId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlan_Id" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlanPaymentDetailId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "NewVendorPlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "EcomUserId" });
            DropIndex("dbo.ActivePlans", new[] { "VendorPlanPaymentDetailId" });
            DropIndex("dbo.ActivePlans", new[] { "EcomUserId" });
            DropIndex("dbo.ActivePlans", new[] { "VendorPlanId" });
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
            DropTable("dbo.Specializations");
            DropTable("dbo.VendorSpecializations");
            DropTable("dbo.VendorPaymentGatewayDetails");
            DropTable("dbo.VendorDetails");
            DropTable("dbo.Specifications");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Products");
            DropTable("dbo.KL_Category");
            DropTable("dbo.CategoryAttributes");
            DropTable("dbo.Categories");
            DropTable("dbo.StoreAddresses");
            DropTable("dbo.Stores");
            DropTable("dbo.Refferals");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Markets");
            DropTable("dbo.Places");
            DropTable("dbo.States");
            DropTable("dbo.Countries");
            DropTable("dbo.Addresses");
            DropTable("dbo.EcomUsers");
            DropTable("dbo.VendorPlanChangeRecords");
            DropTable("dbo.VendorPlans");
            DropTable("dbo.VendorPlanPaymentDetails");
            DropTable("dbo.ActivePlans");
        }
    }
}
