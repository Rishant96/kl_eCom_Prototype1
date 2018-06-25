namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intNullableParentCat2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            AlterColumn("dbo.Categories", "CategoryId", c => c.Int());
            CreateIndex("dbo.Categories", "CategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            AlterColumn("dbo.Categories", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Categories", "CategoryId");
        }
    }
}
