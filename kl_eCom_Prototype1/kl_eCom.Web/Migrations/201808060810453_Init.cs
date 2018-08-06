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
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        VendorPlanId = c.Int(nullable: false),
                        IsPaidFor = c.Boolean(),
                        VendorPaymentDetailsId = c.Int(),
                        PaymentDetails_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPaymentDetails", t => t.PaymentDetails_Id)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorPlanId)
                .Index(t => t.PaymentDetails_Id);
            
            CreateTable(
                "dbo.VendorPaymentDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentMode = c.String(nullable: false),
                        Details = c.String(),
                        VendorPlanId = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.VendorPlanId)
                .Index(t => t.ApplicationUserId);
            
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
                        ValidityPeriod = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        PrimaryRole = c.String(nullable: false),
                        VendorDetailsId = c.Int(),
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
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId)
                .Index(t => t.VendorDetailsId)
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
                "dbo.Refferals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.String(nullable: false, maxLength: 128),
                        VendorId = c.String(nullable: false, maxLength: 128),
                        IsRegisteredUser = c.Boolean(),
                        IsBuyer = c.Boolean(),
                        DateBuyerAdded = c.DateTime(),
                        DateOfRegistration = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CustomerId)
                .ForeignKey("dbo.AspNetUsers", t => t.VendorId)
                .Index(t => t.CustomerId)
                .Index(t => t.VendorId);
            
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
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StoreAddressId = c.Int(nullable: false),
                        DefaultCurrencyType = c.String(),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoreAddresses", t => t.StoreAddressId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.StoreAddressId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.StoreAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Line1 = c.String(nullable: false),
                        Line2 = c.String(),
                        Line3 = c.String(),
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
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: false)
                .Index(t => t.Name, unique: true)
                .Index(t => t.ApplicationUserId)
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
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
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
                        ApplicationUserId = c.String(maxLength: 128),
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
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.OrderId)
                .Index(t => t.StockId)
                .Index(t => t.DiscountConstraintId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        AddressId = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        TotalCost = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: false)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.PlanChangeRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        VendorPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        DecisionDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorPlanId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PlanChangeRequests", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PlanChangeRequests", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.OrderStateInfoes", "OrderItemId", "dbo.OrderItems");
            DropForeignKey("dbo.OrderItems", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.OrderItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.OrderStateInfoes", "RefferalId", "dbo.Refferals");
            DropForeignKey("dbo.CartItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.CartItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.Carts", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropForeignKey("dbo.BundledItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.DiscountConstraints", "DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.Discounts", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Discounts", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.DiscountedItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.Stocks", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Stocks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.DiscountedItems", "DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.BundledItems", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropForeignKey("dbo.ActivePlans", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivePlans", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.ActivePlans", "PaymentDetails_Id", "dbo.VendorPaymentDetails");
            DropForeignKey("dbo.VendorPaymentDetails", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.VendorDetails", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.Stores", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Categories", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Specifications", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.CategoryAttributes", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Stores", "StoreAddressId", "dbo.StoreAddresses");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Refferals", "VendorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Refferals", "CustomerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorPaymentDetails", "VendorPlanId", "dbo.VendorPlans");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PlanChangeRequests", new[] { "VendorPlanId" });
            DropIndex("dbo.PlanChangeRequests", new[] { "ApplicationUserId" });
            DropIndex("dbo.Orders", new[] { "AddressId" });
            DropIndex("dbo.Orders", new[] { "ApplicationUserId" });
            DropIndex("dbo.OrderItems", new[] { "ApplicationUserId" });
            DropIndex("dbo.OrderItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.OrderItems", new[] { "StockId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderStateInfoes", new[] { "RefferalId" });
            DropIndex("dbo.OrderStateInfoes", new[] { "OrderItemId" });
            DropIndex("dbo.Carts", new[] { "ApplicationUserId" });
            DropIndex("dbo.CartItems", new[] { "CartId" });
            DropIndex("dbo.CartItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.CartItems", new[] { "StockId" });
            DropIndex("dbo.Stocks", new[] { "StoreId" });
            DropIndex("dbo.Stocks", new[] { "ProductId" });
            DropIndex("dbo.DiscountedItems", new[] { "StockId" });
            DropIndex("dbo.DiscountedItems", new[] { "DiscountId" });
            DropIndex("dbo.Discounts", new[] { "StoreId" });
            DropIndex("dbo.Discounts", new[] { "ApplicationUserId" });
            DropIndex("dbo.Discounts", new[] { "Name" });
            DropIndex("dbo.DiscountConstraints", new[] { "DiscountId" });
            DropIndex("dbo.BundledItems", new[] { "StockId" });
            DropIndex("dbo.BundledItems", new[] { "DiscountConstraintId" });
            DropIndex("dbo.VendorDetails", new[] { "ActivePlanId" });
            DropIndex("dbo.Specifications", new[] { "ProductId" });
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.CategoryAttributes", new[] { "CategoryId" });
            DropIndex("dbo.Categories", new[] { "StoreId" });
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            DropIndex("dbo.Stores", new[] { "ApplicationUserId" });
            DropIndex("dbo.Stores", new[] { "StoreAddressId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.Refferals", new[] { "VendorId" });
            DropIndex("dbo.Refferals", new[] { "CustomerId" });
            DropIndex("dbo.Addresses", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "VendorDetailsId" });
            DropIndex("dbo.VendorPaymentDetails", new[] { "ApplicationUserId" });
            DropIndex("dbo.VendorPaymentDetails", new[] { "VendorPlanId" });
            DropIndex("dbo.ActivePlans", new[] { "PaymentDetails_Id" });
            DropIndex("dbo.ActivePlans", new[] { "VendorPlanId" });
            DropIndex("dbo.ActivePlans", new[] { "ApplicationUserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PlanChangeRequests");
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
            DropTable("dbo.VendorDetails");
            DropTable("dbo.Specifications");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Products");
            DropTable("dbo.CategoryAttributes");
            DropTable("dbo.Categories");
            DropTable("dbo.StoreAddresses");
            DropTable("dbo.Stores");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Refferals");
            DropTable("dbo.Addresses");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.VendorPlans");
            DropTable("dbo.VendorPaymentDetails");
            DropTable("dbo.ActivePlans");
        }
    }
}
