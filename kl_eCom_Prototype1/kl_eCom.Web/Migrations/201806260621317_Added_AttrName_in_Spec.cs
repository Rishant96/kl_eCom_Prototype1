namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_AttrName_in_Spec : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Name");
        }
    }
}
