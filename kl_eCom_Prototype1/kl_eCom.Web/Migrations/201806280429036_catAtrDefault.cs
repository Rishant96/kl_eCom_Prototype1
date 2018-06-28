namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class catAtrDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryAttributes", "Default", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryAttributes", "Default");
        }
    }
}
