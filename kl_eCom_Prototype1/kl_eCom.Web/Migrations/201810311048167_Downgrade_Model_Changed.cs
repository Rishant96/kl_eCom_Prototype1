namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Downgrade_Model_Changed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorPlanDowngradeRecords", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorPlanDowngradeRecords", "Status");
        }
    }
}
