namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class company_logo_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorDetails", "Logo_Img_Data", c => c.Binary());
            DropColumn("dbo.VendorDetails", "Logo_Path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorDetails", "Logo_Path", c => c.String());
            DropColumn("dbo.VendorDetails", "Logo_Img_Data");
        }
    }
}
