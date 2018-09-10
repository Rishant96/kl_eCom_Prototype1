namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KL_Category_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KL_Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        KL_CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KL_Category", t => t.KL_CategoryId)
                .Index(t => t.KL_CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KL_Category", "KL_CategoryId", "dbo.KL_Category");
            DropIndex("dbo.KL_Category", new[] { "KL_CategoryId" });
            DropTable("dbo.KL_Category");
        }
    }
}
