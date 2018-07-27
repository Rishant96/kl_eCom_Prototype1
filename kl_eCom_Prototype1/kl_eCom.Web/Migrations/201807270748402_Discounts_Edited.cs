namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Discounts_Edited : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DiscountConstraints", "DiscountId", "dbo.Discounts");
            DropIndex("dbo.DiscountConstraints", new[] { "DiscountId" });
            AddColumn("dbo.Discounts", "IsPercent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Discounts", "Value", c => c.Single(nullable: false));
            AddColumn("dbo.Discounts", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.Discounts", "DiscountConstraintId", c => c.Int(nullable: false));
            CreateIndex("dbo.Discounts", "StoreId");
            CreateIndex("dbo.Discounts", "DiscountConstraintId");
            AddForeignKey("dbo.Discounts", "DiscountConstraintId", "dbo.DiscountConstraints", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Discounts", "StoreId", "dbo.Stores", "Id", cascadeDelete: false);
            DropColumn("dbo.DiscountConstraints", "DiscountId");
            DropColumn("dbo.Discounts", "PercentDiscount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Discounts", "PercentDiscount", c => c.Single(nullable: false));
            AddColumn("dbo.DiscountConstraints", "DiscountId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Discounts", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Discounts", "DiscountConstraintId", "dbo.DiscountConstraints");
            DropIndex("dbo.Discounts", new[] { "DiscountConstraintId" });
            DropIndex("dbo.Discounts", new[] { "StoreId" });
            DropColumn("dbo.Discounts", "DiscountConstraintId");
            DropColumn("dbo.Discounts", "StoreId");
            DropColumn("dbo.Discounts", "Value");
            DropColumn("dbo.Discounts", "IsPercent");
            CreateIndex("dbo.DiscountConstraints", "DiscountId");
            AddForeignKey("dbo.DiscountConstraints", "DiscountId", "dbo.Discounts", "Id", cascadeDelete: true);
        }
    }
}
