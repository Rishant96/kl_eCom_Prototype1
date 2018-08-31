namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_IsActive_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EcomUsers", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EcomUsers", "IsActive");
        }
    }
}
