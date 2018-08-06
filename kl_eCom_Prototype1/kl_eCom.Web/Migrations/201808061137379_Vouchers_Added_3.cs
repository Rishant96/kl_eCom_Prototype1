namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Vouchers_Added_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RedeemedVouchers", "DateRedeemed", c => c.DateTime(nullable: false));
            AddColumn("dbo.RedeemedVouchers", "ValueSaved", c => c.Int(nullable: false));
            AddColumn("dbo.RedeemedVouchers", "TimesAvailed", c => c.Int(nullable: false));
            AddColumn("dbo.Vouchers", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.RedeemedVouchers", "TimeRedeemed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RedeemedVouchers", "TimeRedeemed", c => c.DateTime(nullable: false));
            DropColumn("dbo.Vouchers", "IsActive");
            DropColumn("dbo.RedeemedVouchers", "TimesAvailed");
            DropColumn("dbo.RedeemedVouchers", "ValueSaved");
            DropColumn("dbo.RedeemedVouchers", "DateRedeemed");
        }
    }
}
