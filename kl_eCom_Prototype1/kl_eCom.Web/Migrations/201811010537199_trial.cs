namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trial : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VendorPlanDowngradeRecords", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorPlanDowngradeRecords", "Status", c => c.Boolean(nullable: false));
        }
    }
}
