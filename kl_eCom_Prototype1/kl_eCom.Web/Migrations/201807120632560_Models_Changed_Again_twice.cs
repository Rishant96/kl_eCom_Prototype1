namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again_twice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorPackages", "IsEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorPackages", "IsEnabled");
        }
    }
}
