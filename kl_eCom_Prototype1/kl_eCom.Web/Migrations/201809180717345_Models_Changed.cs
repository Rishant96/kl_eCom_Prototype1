namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Markets", "Longitude", c => c.Single());
            DropColumn("dbo.Markets", "Longitutde");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Markets", "Longitutde", c => c.Single());
            DropColumn("dbo.Markets", "Longitude");
        }
    }
}
