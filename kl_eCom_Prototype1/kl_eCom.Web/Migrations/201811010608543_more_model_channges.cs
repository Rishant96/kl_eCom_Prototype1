namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class more_model_channges : DbMigration
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
