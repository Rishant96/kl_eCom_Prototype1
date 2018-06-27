namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SortFilter_ModelSupport_Added : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartItems", "StockId", "dbo.Stocks");
            DropPrimaryKey("dbo.Stocks");
            AddColumn("dbo.CategoryAttributes", "InfoType", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Rating", c => c.Single(nullable: false));
            AddColumn("dbo.Stocks", "StockingDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Stocks", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Stocks", "Id");
            AddForeignKey("dbo.CartItems", "StockId", "dbo.Stocks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "StockId", "dbo.Stocks");
            DropPrimaryKey("dbo.Stocks");
            AlterColumn("dbo.Stocks", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Stocks", "StockingDate");
            DropColumn("dbo.Products", "Rating");
            DropColumn("dbo.CategoryAttributes", "InfoType");
            AddPrimaryKey("dbo.Stocks", "Id");
            AddForeignKey("dbo.CartItems", "StockId", "dbo.Stocks", "Id", cascadeDelete: true);
        }
    }
}
