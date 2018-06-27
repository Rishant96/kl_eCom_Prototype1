namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Product_AutoGen_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Specifications", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Stocks", "ProductId", "dbo.Products");
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Products", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Products", "DateAdded", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.Products", "Id");
            AddForeignKey("dbo.Specifications", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Stocks", "ProductId", "dbo.Products", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Specifications", "ProductId", "dbo.Products");
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Products", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Products", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Products", "Id");
            AddForeignKey("dbo.Stocks", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Specifications", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
        }
    }
}
