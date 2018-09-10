namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KL_Category", "Description", c => c.String());
            AddColumn("dbo.KL_Category", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KL_Category", "ImagePath");
            DropColumn("dbo.KL_Category", "Description");
        }
    }
}
