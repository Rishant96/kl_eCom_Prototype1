namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class model_changed_3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VendorPaymentGatewayDetails", "ProviderName", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VendorPaymentGatewayDetails", "ProviderName", c => c.String());
        }
    }
}
