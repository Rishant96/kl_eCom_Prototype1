namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartModelEdited : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Carts", "TotalCost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carts", "TotalCost", c => c.Single(nullable: false));
        }
    }
}
