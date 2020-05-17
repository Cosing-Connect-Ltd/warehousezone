namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitecartandwishlistTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteCartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.ProductId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WebsiteWishListItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        IsNotification = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteWishListItems", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteWishListItems", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.WebsiteWishListItems", "UserId", "dbo.AuthUsers");
            DropForeignKey("dbo.WebsiteCartItems", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteCartItems", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.WebsiteCartItems", "UserId", "dbo.AuthUsers");
            DropIndex("dbo.WebsiteWishListItems", new[] { "ProductId" });
            DropIndex("dbo.WebsiteWishListItems", new[] { "UserId" });
            DropIndex("dbo.WebsiteWishListItems", new[] { "SiteID" });
            DropIndex("dbo.WebsiteCartItems", new[] { "UserId" });
            DropIndex("dbo.WebsiteCartItems", new[] { "ProductId" });
            DropIndex("dbo.WebsiteCartItems", new[] { "SiteID" });
            DropTable("dbo.WebsiteWishListItems");
            DropTable("dbo.WebsiteCartItems");
        }
    }
}
