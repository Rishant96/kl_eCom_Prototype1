namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment_models_changed : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanId", "dbo.VendorPlans");
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "ActivePlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlanId" });
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "VendorPlanId", newName: "NewPlan_Id");
            AddColumn("dbo.VendorPlanDowngradeRecords", "IsPending", c => c.Boolean(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.VendorPlanChangeRecords", "OldPlanName", c => c.String(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "NewPlanName", c => c.String(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldBalance", c => c.Single(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId", c => c.Int(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldPlan_Id", c => c.Int());
            AlterColumn("dbo.VendorPlanChangeRecords", "NewPlan_Id", c => c.Int());
            CreateIndex("dbo.VendorPlanChangeRecords", "NewPlan_Id");
            CreateIndex("dbo.VendorPlanChangeRecords", "OldPlan_Id");
            AddForeignKey("dbo.VendorPlanChangeRecords", "OldPlan_Id", "dbo.VendorPlans", "Id");
            AddForeignKey("dbo.VendorPlanChangeRecords", "NewPlan_Id", "dbo.VendorPlans", "Id");
            DropColumn("dbo.VendorPlanDowngradeRecords", "ActivePlanId");
            DropColumn("dbo.VendorPlanChangeRecords", "StartDate");
            DropColumn("dbo.VendorPlanChangeRecords", "PlanName");
            DropColumn("dbo.VendorPlanChangeRecords", "Balance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorPlanChangeRecords", "Balance", c => c.Single(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "PlanName", c => c.String());
            AddColumn("dbo.VendorPlanChangeRecords", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.VendorPlanDowngradeRecords", "ActivePlanId", c => c.Int(nullable: false));
            DropForeignKey("dbo.VendorPlanChangeRecords", "NewPlan_Id", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "OldPlan_Id", "dbo.VendorPlans");
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "OldPlan_Id" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "NewPlan_Id" });
            AlterColumn("dbo.VendorPlanChangeRecords", "NewPlan_Id", c => c.Int(nullable: false));
            DropColumn("dbo.VendorPlanChangeRecords", "OldPlan_Id");
            DropColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId");
            DropColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId");
            DropColumn("dbo.VendorPlanChangeRecords", "OldBalance");
            DropColumn("dbo.VendorPlanChangeRecords", "NewPlanName");
            DropColumn("dbo.VendorPlanChangeRecords", "OldPlanName");
            DropColumn("dbo.VendorPlanChangeRecords", "OldStartDate");
            DropColumn("dbo.VendorPlanDowngradeRecords", "IsPending");
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "NewPlan_Id", newName: "VendorPlanId");
            CreateIndex("dbo.VendorPlanChangeRecords", "VendorPlanId");
            CreateIndex("dbo.VendorPlanDowngradeRecords", "ActivePlanId");
            AddForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanId", "dbo.VendorPlans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans", "Id", cascadeDelete: true);
        }
    }
}
