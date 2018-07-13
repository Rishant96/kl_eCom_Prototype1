namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again_5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorDetails", "ActivePackageId", "dbo.ActivePackages");
            DropIndex("dbo.VendorDetails", new[] { "ActivePackageId" });
            AlterColumn("dbo.VendorDetails", "ActivePackageId", c => c.Int());
            CreateIndex("dbo.VendorDetails", "ActivePackageId");
            AddForeignKey("dbo.VendorDetails", "ActivePackageId", "dbo.ActivePackages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorDetails", "ActivePackageId", "dbo.ActivePackages");
            DropIndex("dbo.VendorDetails", new[] { "ActivePackageId" });
            AlterColumn("dbo.VendorDetails", "ActivePackageId", c => c.Int(nullable: false));
            CreateIndex("dbo.VendorDetails", "ActivePackageId");
            AddForeignKey("dbo.VendorDetails", "ActivePackageId", "dbo.ActivePackages", "Id", cascadeDelete: true);
        }
    }
}
