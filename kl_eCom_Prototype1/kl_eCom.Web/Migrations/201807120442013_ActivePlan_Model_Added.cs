namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivePlan_Model_Added : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorDetails", "Package_Id", "dbo.VendorPackages");
            DropIndex("dbo.VendorDetails", new[] { "Package_Id" });
            CreateTable(
                "dbo.ActivePackages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        VendorPackageId = c.Int(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPackages", t => t.VendorPackageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.VendorPackageId);
            
            AddColumn("dbo.VendorDetails", "ActivePackage_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.VendorDetails", "ActivePackage_Id");
            AddForeignKey("dbo.VendorDetails", "ActivePackage_Id", "dbo.ActivePackages", "Id", cascadeDelete: true);
            DropColumn("dbo.VendorDetails", "Package_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorDetails", "Package_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.VendorDetails", "ActivePackage_Id", "dbo.ActivePackages");
            DropForeignKey("dbo.ActivePackages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivePackages", "VendorPackageId", "dbo.VendorPackages");
            DropIndex("dbo.ActivePackages", new[] { "VendorPackageId" });
            DropIndex("dbo.ActivePackages", new[] { "ApplicationUserId" });
            DropIndex("dbo.VendorDetails", new[] { "ActivePackage_Id" });
            DropColumn("dbo.VendorDetails", "ActivePackage_Id");
            DropTable("dbo.ActivePackages");
            CreateIndex("dbo.VendorDetails", "Package_Id");
            AddForeignKey("dbo.VendorDetails", "Package_Id", "dbo.VendorPackages", "Id", cascadeDelete: true);
        }
    }
}
