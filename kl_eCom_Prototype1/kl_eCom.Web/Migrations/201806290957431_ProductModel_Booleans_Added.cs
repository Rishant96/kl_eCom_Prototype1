namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductModel_Booleans_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsCategoryListable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "HasStock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "HasStock");
            DropColumn("dbo.Products", "IsCategoryListable");
        }
    }
}
