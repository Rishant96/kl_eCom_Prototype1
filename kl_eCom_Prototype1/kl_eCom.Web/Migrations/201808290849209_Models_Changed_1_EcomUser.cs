namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Changed_1_EcomUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stores", "EcomUser_Id", "dbo.EcomUsers");
            DropIndex("dbo.Stores", new[] { "ApplicationUserId" });
            DropIndex("dbo.Stores", new[] { "EcomUser_Id" });
            RenameColumn(table: "dbo.Stores", name: "ApplicationUserId", newName: "EcomUserId");
            RenameColumn(table: "dbo.Stores", name: "EcomUser_Id", newName: "EcomUserId");
            AlterColumn("dbo.Stores", "EcomUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Stores", "EcomUserId");
            AddForeignKey("dbo.Stores", "EcomUserId", "dbo.EcomUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stores", "EcomUserId", "dbo.EcomUsers");
            DropIndex("dbo.Stores", new[] { "EcomUserId" });
            AlterColumn("dbo.Stores", "EcomUserId", c => c.Int());
            RenameColumn(table: "dbo.Stores", name: "EcomUserId", newName: "EcomUser_Id");
            RenameColumn(table: "dbo.Stores", name: "EcomUserId", newName: "ApplicationUserId");
            CreateIndex("dbo.Stores", "EcomUser_Id");
            CreateIndex("dbo.Stores", "ApplicationUserId");
            AddForeignKey("dbo.Stores", "EcomUser_Id", "dbo.EcomUsers", "Id");
        }
    }
}
