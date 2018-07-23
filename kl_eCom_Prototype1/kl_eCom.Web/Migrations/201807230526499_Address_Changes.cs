namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address_Changes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Addresses", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Addresses", new[] { "User_Id" });
            DropColumn("dbo.Addresses", "ApplicationUserId");
            RenameColumn(table: "dbo.Addresses", name: "User_Id", newName: "ApplicationUserId");
            AddColumn("dbo.Addresses", "City", c => c.String(nullable: false));
            AddColumn("dbo.Addresses", "Place", c => c.String());
            AlterColumn("dbo.Addresses", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Addresses", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Addresses", "ApplicationUserId");
            AddForeignKey("dbo.Addresses", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Addresses", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Addresses", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Addresses", "ApplicationUserId", c => c.Int(nullable: false));
            DropColumn("dbo.Addresses", "Place");
            DropColumn("dbo.Addresses", "City");
            RenameColumn(table: "dbo.Addresses", name: "ApplicationUserId", newName: "User_Id");
            AddColumn("dbo.Addresses", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Addresses", "User_Id");
            AddForeignKey("dbo.Addresses", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
