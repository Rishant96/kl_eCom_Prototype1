namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment_models_changed_2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "NewPlan_Id" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "OldPlan_Id" });
            DropColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId");
            DropColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId");
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "NewPlan_Id", newName: "NewVendorPlanId");
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "OldPlan_Id", newName: "OldVendorPlanId");
            AlterColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int(nullable: true));
            AlterColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId", c => c.Int(nullable: true));
            CreateIndex("dbo.VendorPlanChangeRecords", "OldVendorPlanId");
            CreateIndex("dbo.VendorPlanChangeRecords", "NewVendorPlanId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "NewVendorPlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "OldVendorPlanId" });
            AlterColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId", c => c.Int());
            AlterColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int());
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "OldVendorPlanId", newName: "OldPlan_Id");
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "NewVendorPlanId", newName: "NewPlan_Id");
            AddColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId", c => c.Int(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int(nullable: false));
            CreateIndex("dbo.VendorPlanChangeRecords", "OldPlan_Id");
            CreateIndex("dbo.VendorPlanChangeRecords", "NewPlan_Id");
        }
    }
}
