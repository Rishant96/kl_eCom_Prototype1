namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_VendorRegistration_Details : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropIndex("dbo.AspNetUsers", new[] { "VendorDetailsId" });
            DropColumn("dbo.AspNetUsers", "VendorDetailsId");
            DropTable("dbo.VendorDetails");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.VendorDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(nullable: false),
                        WebsiteUrl = c.String(),
                        Zip = c.String(nullable: false),
                        State = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "VendorDetailsId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "VendorDetailsId");
            AddForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails", "Id");
        }
    }
}
