namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_updated_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanChangeRequests", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlanChangeRequests", "Date");
        }
    }
}
