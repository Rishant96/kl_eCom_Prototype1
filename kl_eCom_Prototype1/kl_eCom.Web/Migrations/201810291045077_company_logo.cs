namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class company_logo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorDetails", "Logo_Path", c => c.String());
            AddColumn("dbo.VendorDetails", "Logo_Mime_Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorDetails", "Logo_Mime_Type");
            DropColumn("dbo.VendorDetails", "Logo_Path");
        }
    }
}
