namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockModel_Status_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "Status");
        }
    }
}
