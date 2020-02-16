namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebsitesProductsRelationTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductsWebsitesMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        SiteID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.ProductId)
                .Index(t => t.SiteID);
            
            AlterColumn("dbo.ProductKitMaps", "CreatedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductsWebsitesMaps", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.ProductsWebsitesMaps", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductsWebsitesMaps", new[] { "SiteID" });
            DropIndex("dbo.ProductsWebsitesMaps", new[] { "ProductId" });
            AlterColumn("dbo.ProductKitMaps", "CreatedBy", c => c.Int(nullable: false));
            DropTable("dbo.ProductsWebsitesMaps");
        }
    }
}
