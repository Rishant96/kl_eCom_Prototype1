namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockModel_Date_Changed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stocks", "StockingDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stocks", "StockingDate", c => c.DateTime(nullable: false));
        }
    }
}
