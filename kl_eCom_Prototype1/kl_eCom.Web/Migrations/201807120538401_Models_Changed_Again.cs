namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VendorPackages", "ValidityPeriod", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VendorPackages", "ValidityPeriod", c => c.Int(nullable: false));
        }
    }
}
