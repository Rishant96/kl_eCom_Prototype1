namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class catAtrDefault2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CategoryAttributes", "Default", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CategoryAttributes", "Default", c => c.String(nullable: false));
        }
    }
}
