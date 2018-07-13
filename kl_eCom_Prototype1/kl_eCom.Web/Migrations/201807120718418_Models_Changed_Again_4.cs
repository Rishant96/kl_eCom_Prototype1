namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_Again_4 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.VendorDetails", name: "ActivePackage_Id", newName: "ActivePackageId");
            RenameIndex(table: "dbo.VendorDetails", name: "IX_ActivePackage_Id", newName: "IX_ActivePackageId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.VendorDetails", name: "IX_ActivePackageId", newName: "IX_ActivePackage_Id");
            RenameColumn(table: "dbo.VendorDetails", name: "ActivePackageId", newName: "ActivePackage_Id");
        }
    }
}
