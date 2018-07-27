namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Discounts_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiscountConstraints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        MinQty = c.Int(),
                        MinOrder = c.Single(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discounts", t => t.DiscountId, cascadeDelete: true)
                .Index(t => t.DiscountId);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        IsExpirable = c.Boolean(nullable: false),
                        ValidityPeriod = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsConstrained = c.Boolean(nullable: false),
                        PercentDiscount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
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
            
            AddColumn("dbo.Stores", "DefaultCurrencyType", c => c.String());
            AddColumn("dbo.Stocks", "CurrencyType", c => c.String());
            AddColumn("dbo.OrderItems", "CurrencyType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Discounts", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DiscountedItems", "StockId", "dbo.Stocks");
            DropForeignKey("dbo.DiscountedItems", "DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.DiscountConstraints", "DiscountId", "dbo.Discounts");
            DropIndex("dbo.DiscountedItems", new[] { "StockId" });
            DropIndex("dbo.DiscountedItems", new[] { "DiscountId" });
            DropIndex("dbo.Discounts", new[] { "ApplicationUserId" });
            DropIndex("dbo.DiscountConstraints", new[] { "DiscountId" });
            DropColumn("dbo.OrderItems", "CurrencyType");
            DropColumn("dbo.Stocks", "CurrencyType");
            DropColumn("dbo.Stores", "DefaultCurrencyType");
            DropTable("dbo.DiscountedItems");
            DropTable("dbo.Discounts");
            DropTable("dbo.DiscountConstraints");
        }
    }
}
