namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Product_and_Category_Images_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageData = c.Binary(),
                        ImageMimeType = c.String(),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.Categories", "ThumbnailData", c => c.Binary());
            AddColumn("dbo.Categories", "ThumbnailMimeType", c => c.String());
            AddColumn("dbo.Products", "ThumbnailData", c => c.Binary());
            AddColumn("dbo.Products", "ThumbnailMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropColumn("dbo.Products", "ThumbnailMimeType");
            DropColumn("dbo.Products", "ThumbnailData");
            DropColumn("dbo.Categories", "ThumbnailMimeType");
            DropColumn("dbo.Categories", "ThumbnailData");
            DropTable("dbo.ProductImages");
        }
    }
}
