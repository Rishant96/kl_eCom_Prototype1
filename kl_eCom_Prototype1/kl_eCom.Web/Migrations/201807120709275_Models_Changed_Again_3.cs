namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again_3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorPaymentDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentMode = c.String(nullable: false),
                        KL_Notes = c.String(),
                        VendorPackageId = c.Int(),
                        ApplicationUserId = c.Int(nullable: false),
                        Vendor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VendorPackages", t => t.VendorPackageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Vendor_Id)
                .Index(t => t.VendorPackageId)
                .Index(t => t.Vendor_Id);
            
            AddColumn("dbo.ActivePackages", "IsPaidFor", c => c.Boolean());
            AddColumn("dbo.ActivePackages", "VendorPaymentDetailsId", c => c.Int());
            CreateIndex("dbo.ActivePackages", "VendorPaymentDetailsId");
            AddForeignKey("dbo.ActivePackages", "VendorPaymentDetailsId", "dbo.VendorPaymentDetails", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivePackages", "VendorPaymentDetailsId", "dbo.VendorPaymentDetails");
            DropForeignKey("dbo.VendorPaymentDetails", "Vendor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorPaymentDetails", "VendorPackageId", "dbo.VendorPackages");
            DropIndex("dbo.VendorPaymentDetails", new[] { "Vendor_Id" });
            DropIndex("dbo.VendorPaymentDetails", new[] { "VendorPackageId" });
            DropIndex("dbo.ActivePackages", new[] { "VendorPaymentDetailsId" });
            DropColumn("dbo.ActivePackages", "VendorPaymentDetailsId");
            DropColumn("dbo.ActivePackages", "IsPaidFor");
            DropTable("dbo.VendorPaymentDetails");
        }
    }
}
