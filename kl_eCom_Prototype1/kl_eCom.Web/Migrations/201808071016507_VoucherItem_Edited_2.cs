namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoucherItem_Edited_2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VoucherItems", "IsProductSpecific");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VoucherItems", "IsProductSpecific", c => c.Boolean(nullable: false));
        }
    }
}
