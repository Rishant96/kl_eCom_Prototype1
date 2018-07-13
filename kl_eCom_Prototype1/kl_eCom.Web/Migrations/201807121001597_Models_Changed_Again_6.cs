namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again_6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanChangeRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        VendorPackageId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPackages", t => t.VendorPackageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorPackageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlanChangeRequests", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PlanChangeRequests", "VendorPackageId", "dbo.VendorPackages");
            DropIndex("dbo.PlanChangeRequests", new[] { "VendorPackageId" });
            DropIndex("dbo.PlanChangeRequests", new[] { "ApplicationUserId" });
            DropTable("dbo.PlanChangeRequests");
        }
    }
}
