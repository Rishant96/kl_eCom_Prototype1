namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartModelsEdited : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carts", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Carts", new[] { "Owner_Id" });
            DropColumn("dbo.Carts", "ApplicationUserId");
            RenameColumn(table: "dbo.Carts", name: "Owner_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.Carts", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Carts", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Carts", "ApplicationUserId");
            AddForeignKey("dbo.Carts", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carts", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Carts", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Carts", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Carts", "ApplicationUserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Carts", name: "ApplicationUserId", newName: "Owner_Id");
            AddColumn("dbo.Carts", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Carts", "Owner_Id");
            AddForeignKey("dbo.Carts", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
