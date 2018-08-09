namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GST_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "DefaultGST", c => c.Single(nullable: false));
            AddColumn("dbo.Products", "DefaultGST", c => c.Single(nullable: false));
            AddColumn("dbo.Stocks", "GST", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "GST");
            DropColumn("dbo.Products", "DefaultGST");
            DropColumn("dbo.Categories", "DefaultGST");
        }
    }
}
