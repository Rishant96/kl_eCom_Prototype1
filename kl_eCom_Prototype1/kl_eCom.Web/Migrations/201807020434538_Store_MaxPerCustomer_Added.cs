namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Store_MaxPerCustomer_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "MaxAmtPerUser", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "MaxAmtPerUser");
        }
    }
}
