namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KL_Category_Added_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "KL_CategoryId", c => c.Int());
            CreateIndex("dbo.Categories", "KL_CategoryId");
            AddForeignKey("dbo.Categories", "KL_CategoryId", "dbo.KL_Category", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "KL_CategoryId", "dbo.KL_Category");
            DropIndex("dbo.Categories", new[] { "KL_CategoryId" });
            DropColumn("dbo.Categories", "KL_CategoryId");
        }
    }
}
