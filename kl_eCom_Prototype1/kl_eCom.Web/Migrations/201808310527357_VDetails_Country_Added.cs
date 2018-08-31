namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VDetails_Country_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorDetails", "Country", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorDetails", "Country");
        }
    }
}
