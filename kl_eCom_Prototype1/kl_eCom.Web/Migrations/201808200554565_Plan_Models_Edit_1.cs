namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Plan_Models_Edit_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorPlanDowngradeRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ActivePlanId = c.Int(nullable: false),
                        VendorPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivePlans", t => t.ActivePlanId, cascadeDelete: true)
                .ForeignKey("dbo.VendorPlans", t => t.VendorPlanId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.ActivePlanId)
                .Index(t => t.VendorPlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "VendorPlanId", "dbo.VendorPlans");
            DropForeignKey("dbo.VendorPlanDowngradeRecords", "ActivePlanId", "dbo.ActivePlans");
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "VendorPlanId" });
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "ActivePlanId" });
            DropIndex("dbo.VendorPlanDowngradeRecords", new[] { "ApplicationUserId" });
            DropTable("dbo.VendorPlanDowngradeRecords");
        }
    }
}
