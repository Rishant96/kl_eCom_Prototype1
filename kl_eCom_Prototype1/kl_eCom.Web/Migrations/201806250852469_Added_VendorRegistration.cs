namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_VendorRegistration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropIndex("dbo.AspNetUsers", new[] { "VendorDetailsId" });
            AlterColumn("dbo.AspNetUsers", "VendorDetailsId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "VendorDetailsId");
            AddForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropIndex("dbo.AspNetUsers", new[] { "VendorDetailsId" });
            AlterColumn("dbo.AspNetUsers", "VendorDetailsId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "VendorDetailsId");
            AddForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails", "Id", cascadeDelete: true);
        }
    }
}
