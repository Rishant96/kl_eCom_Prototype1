namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Additional_Order_Details_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderStateInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderItemId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        InitialDate = c.DateTime(nullable: false),
                        FinalDate = c.DateTime(),
                        IsChangePostive = c.Boolean(),
                        RefferalId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Refferals", t => t.RefferalId, cascadeDelete: true)
                .ForeignKey("dbo.OrderItems", t => t.OrderItemId, cascadeDelete: true)
                .Index(t => t.OrderItemId)
                .Index(t => t.RefferalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderStateInfoes", "OrderItemId", "dbo.OrderItems");
            DropForeignKey("dbo.OrderStateInfoes", "RefferalId", "dbo.Refferals");
            DropIndex("dbo.OrderStateInfoes", new[] { "RefferalId" });
            DropIndex("dbo.OrderStateInfoes", new[] { "OrderItemId" });
            DropTable("dbo.OrderStateInfoes");
        }
    }
}
