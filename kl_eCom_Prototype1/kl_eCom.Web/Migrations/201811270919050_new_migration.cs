namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new_migration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanId", "dbo.VendorPlans");
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "ActivePlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "VendorPlanId" });
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "VendorPlanId", newName: "NewVendorPlanId");
            AddColumn("dbo.VendorPlanDowngradeRecords", "IsPending", c => c.Boolean(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.VendorPlanChangeRecords", "OldPlanName", c => c.String(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "NewPlanName", c => c.String(nullable: false));
            AddColumn("dbo.VendorPlanChangeRecords", "OldBalance", c => c.Single(nullable: false));
            AlterColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int());
            CreateIndex("dbo.VendorPlanChangeRecords", "NewVendorPlanId");
            AddForeignKey("dbo.VendorPlanChangeRecords", "NewVendorPlanId", "dbo.VendorPlans", "Id");
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
            DropForeignKey("dbo.VendorPlanChangeRecords", "NewVendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanChangeRecords", "OldVendorPlanId", "dbo.VendorPlans");
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "NewVendorPlanId" });
            DropIndex("dbo.VendorPlanChangeRecords", new[] { "OldVendorPlanId" });
            AlterColumn("dbo.VendorPlanChangeRecords", "NewVendorPlanId", c => c.Int(nullable: false));
            DropColumn("dbo.VendorPlanChangeRecords", "OldVendorPlanId");
            DropColumn("dbo.VendorPlanChangeRecords", "OldBalance");
            DropColumn("dbo.VendorPlanChangeRecords", "NewPlanName");
            DropColumn("dbo.VendorPlanChangeRecords", "OldPlanName");
            DropColumn("dbo.VendorPlanChangeRecords", "OldStartDate");
            DropColumn("dbo.VendorPlanDowngradeRecords", "IsPending");
            RenameColumn(table: "dbo.VendorPlanChangeRecords", name: "NewVendorPlanId", newName: "VendorPlanId");
            CreateIndex("dbo.VendorPlanChangeRecords", "VendorPlanId");
            CreateIndex("dbo.VendorPlanDowngradeRecords", "ActivePlanId");
            AddForeignKey("dbo.VendorPlanChangeRecords", "VendorPlanId", "dbo.VendorPlans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans", "Id", cascadeDelete: true);
        }
    }
}
