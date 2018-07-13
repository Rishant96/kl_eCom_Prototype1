namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorPackage_Model_Edited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorPackages", "MaxProducts", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorPackages", "MaxProducts");
        }
    }
}
