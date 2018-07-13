namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_updated_3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActivePackages", "VendorPaymentDetailsId", "dbo.VendorPaymentDetails");
            DropIndex("dbo.ActivePackages", new[] { "VendorPaymentDetailsId" });
            AddColumn("dbo.ActivePackages", "PaymentDetails_Id", c => c.Int());
            AddColumn("dbo.VendorDetails", "DomainRegistrationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PlanChangeRequests", "RequestDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PlanChangeRequests", "DecisionDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.ActivePackages", "PaymentDetails_Id");
            AddForeignKey("dbo.ActivePackages", "PaymentDetails_Id", "dbo.VendorPaymentDetails", "Id");
            DropColumn("dbo.ActivePackages", "ExpiryDate");
            DropColumn("dbo.PlanChangeRequests", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlanChangeRequests", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.ActivePackages", "ExpiryDate", c => c.DateTime());
            DropForeignKey("dbo.ActivePackages", "PaymentDetails_Id", "dbo.VendorPaymentDetails");
            DropIndex("dbo.ActivePackages", new[] { "PaymentDetails_Id" });
            DropColumn("dbo.PlanChangeRequests", "DecisionDate");
            DropColumn("dbo.PlanChangeRequests", "RequestDate");
            DropColumn("dbo.VendorDetails", "DomainRegistrationDate");
            DropColumn("dbo.ActivePackages", "PaymentDetails_Id");
            CreateIndex("dbo.ActivePackages", "VendorPaymentDetailsId");
            AddForeignKey("dbo.ActivePackages", "VendorPaymentDetailsId", "dbo.VendorPaymentDetails", "Id");
        }
    }
}
