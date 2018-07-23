namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Thumbnail_Model_Edited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ThumbnailPath", c => c.String());
            AddColumn("dbo.ProductImages", "ImagePath", c => c.String());
            DropColumn("dbo.Products", "ThumbnailData");
            DropColumn("dbo.ProductImages", "ImageData");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductImages", "ImageData", c => c.Binary());
            AddColumn("dbo.Products", "ThumbnailData", c => c.Binary());
            DropColumn("dbo.ProductImages", "ImagePath");
            DropColumn("dbo.Products", "ThumbnailPath");
        }
    }
}
