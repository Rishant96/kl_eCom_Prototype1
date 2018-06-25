namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_VendorRegistration_Details : DbMigration
    {
        public override void Up()
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
            
            AddColumn("dbo.AspNetUsers", "VendorDetailsId", c => c.Int(nullable: true));
            CreateIndex("dbo.AspNetUsers", "VendorDetailsId");
            AddForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "VendorDetailsId", "dbo.VendorDetails");
            DropIndex("dbo.AspNetUsers", new[] { "VendorDetailsId" });
            DropColumn("dbo.AspNetUsers", "VendorDetailsId");
            DropTable("dbo.VendorDetails");
        }
    }
}
