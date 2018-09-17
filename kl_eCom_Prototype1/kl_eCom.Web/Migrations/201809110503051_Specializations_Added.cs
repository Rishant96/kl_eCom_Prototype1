namespace kl_eCom.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Specializations_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorSpecializations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SpecializationId = c.Int(nullable: false),
                        VendorDetailsId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specializations", t => t.SpecializationId, cascadeDelete: true)
                .ForeignKey("dbo.VendorDetails", t => t.VendorDetailsId, cascadeDelete: true)
                .Index(t => t.SpecializationId)
                .Index(t => t.VendorDetailsId);
            
            CreateTable(
                "dbo.Specializations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SpecializationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specializations", t => t.SpecializationId)
                .Index(t => t.SpecializationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorSpecializations", "VendorDetailsId", "dbo.VendorDetails");
            DropForeignKey("dbo.Specializations", "SpecializationId", "dbo.Specializations");
            DropForeignKey("dbo.VendorSpecializations", "SpecializationId", "dbo.Specializations");
            DropIndex("dbo.Specializations", new[] { "SpecializationId" });
            DropIndex("dbo.VendorSpecializations", new[] { "VendorDetailsId" });
            DropIndex("dbo.VendorSpecializations", new[] { "SpecializationId" });
            DropTable("dbo.Specializations");
            DropTable("dbo.VendorSpecializations");
        }
    }
}
