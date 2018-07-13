namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_updated_1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorPaymentDetails", "VendorPackageId", "dbo.VendorPackages");
            DropIndex("dbo.VendorPaymentDetails", new[] { "VendorPackageId" });
            DropIndex("dbo.VendorPaymentDetails", new[] { "Vendor_Id" });
            DropColumn("dbo.VendorPaymentDetails", "ApplicationUserId");
            RenameColumn(table: "dbo.VendorPaymentDetails", name: "Vendor_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.VendorPaymentDetails", "VendorPackageId", c => c.Int(nullable: false));
            AlterColumn("dbo.VendorPaymentDetails", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.VendorPaymentDetails", "VendorPackageId");
            CreateIndex("dbo.VendorPaymentDetails", "ApplicationUserId");
            AddForeignKey("dbo.VendorPaymentDetails", "VendorPackageId", "dbo.VendorPackages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorPaymentDetails", "VendorPackageId", "dbo.VendorPackages");
            DropIndex("dbo.VendorPaymentDetails", new[] { "ApplicationUserId" });
            DropIndex("dbo.VendorPaymentDetails", new[] { "VendorPackageId" });
            AlterColumn("dbo.VendorPaymentDetails", "ApplicationUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.VendorPaymentDetails", "VendorPackageId", c => c.Int());
            RenameColumn(table: "dbo.VendorPaymentDetails", name: "ApplicationUserId", newName: "Vendor_Id");
            AddColumn("dbo.VendorPaymentDetails", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.VendorPaymentDetails", "Vendor_Id");
            CreateIndex("dbo.VendorPaymentDetails", "VendorPackageId");
            AddForeignKey("dbo.VendorPaymentDetails", "VendorPackageId", "dbo.VendorPackages", "Id");
        }
    }
}
