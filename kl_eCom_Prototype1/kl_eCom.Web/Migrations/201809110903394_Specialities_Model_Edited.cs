namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Specialities_Model_Edited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specializations", "IsVisible", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specializations", "IsVisible");
        }
    }
}
