namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_41 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VendorSpecializations", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorSpecializations", "Description", c => c.String());
        }
    }
}
