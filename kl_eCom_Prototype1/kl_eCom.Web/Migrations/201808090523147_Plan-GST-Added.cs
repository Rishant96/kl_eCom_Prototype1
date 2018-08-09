namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanGSTAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorPlans", "GST", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorPlans", "GST");
        }
    }
}
