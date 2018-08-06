namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Vouchers_Added_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RedeemedVouchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeRedeemed = c.DateTime(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        VoucherId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: false)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
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
                        IsAutomatic = c.Boolean(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        MaxAvailPerCustomer = c.Int(),
                        Value = c.Single(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.VoucherItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoucherId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        IsProductSpecific = c.Boolean(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RedeemedVouchers", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherItems", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.VoucherItems", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Vouchers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RedeemedVouchers", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.VoucherItems", new[] { "StockId" });
            DropIndex("dbo.VoucherItems", new[] { "CategoryId" });
            DropIndex("dbo.VoucherItems", new[] { "VoucherId" });
            DropIndex("dbo.Vouchers", new[] { "ApplicationUserId" });
            DropIndex("dbo.RedeemedVouchers", new[] { "VoucherId" });
            DropIndex("dbo.RedeemedVouchers", new[] { "ApplicationUserId" });
            DropTable("dbo.VoucherItems");
            DropTable("dbo.Vouchers");
            DropTable("dbo.RedeemedVouchers");
        }
    }
}
