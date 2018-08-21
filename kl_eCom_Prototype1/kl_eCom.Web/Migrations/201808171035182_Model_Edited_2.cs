namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Model_Edited_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorPlanPaymentDetails", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorPlanPaymentDetails", "Notes");
        }
    }
}
