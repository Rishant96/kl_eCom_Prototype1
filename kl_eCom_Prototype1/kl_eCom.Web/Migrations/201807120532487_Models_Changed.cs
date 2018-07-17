namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActivePackages", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ActivePackages", new[] { "ApplicationUserId" });
            AlterColumn("dbo.ActivePackages", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ActivePackages", "ExpiryDate", c => c.DateTime());
            CreateIndex("dbo.ActivePackages", "ApplicationUserId");
            AddForeignKey("dbo.ActivePackages", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivePackages", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ActivePackages", new[] { "ApplicationUserId" });
            AlterColumn("dbo.ActivePackages", "ExpiryDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ActivePackages", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ActivePackages", "ApplicationUserId");
            AddForeignKey("dbo.ActivePackages", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
