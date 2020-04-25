namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitetablesadded : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.TenantWebsites", name: "WarehouseId", newName: "DefaultWarehouseId");
            RenameIndex(table: "dbo.TenantWebsites", name: "IX_WarehouseId", newName: "IX_DefaultWarehouseId");
            CreateTable(
                "dbo.ProductsNavigationMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductWebsiteMapId = c.Int(nullable: false),
                        NavigationId = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductsWebsitesMaps", t => t.ProductWebsiteMapId)
                .ForeignKey("dbo.WebsiteNavigations", t => t.NavigationId)
                .Index(t => t.ProductWebsiteMapId)
                .Index(t => t.NavigationId);
            
            CreateTable(
                "dbo.WebsiteNavigations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        Image = c.String(),
                        IamgeAltTag = c.String(),
                        HoverImage = c.String(),
                        HoverIamgeAltTag = c.String(),
                        Name = c.String(),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        Type = c.Int(nullable: false),
                        ContentPageId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebsiteNavigations", t => t.ParentId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.WebsiteContentPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        Title = c.String(),
                        MetaTitle = c.String(),
                        MetaDescription = c.String(),
                        Contant = c.String(),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID);
            
            CreateTable(
                "dbo.WebsiteSliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        Image = c.String(),
                        IamgeAltTag = c.String(),
                        Text = c.String(),
                        ButtonText = c.String(),
                        ButtonLinkUrl = c.String(),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID);
            
            AddColumn("dbo.TenantWebsites", "HostName", c => c.String());
            AddColumn("dbo.TenantWebsites", "Logo", c => c.String());
            AddColumn("dbo.TenantWebsites", "FacebookUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "TwitterUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "LinkedInUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "YoutubeUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "InstaGramUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "FooterText", c => c.String());
            AddColumn("dbo.TenantWebsites", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductsWebsitesMaps", "SortOrder", c => c.Int(nullable: false));
            AddColumn("dbo.ProductsWebsitesMaps", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteSliders", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteContentPages", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteNavigations", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.ProductsNavigationMaps", "NavigationId", "dbo.WebsiteNavigations");
            DropForeignKey("dbo.WebsiteNavigations", "ParentId", "dbo.WebsiteNavigations");
            DropForeignKey("dbo.ProductsNavigationMaps", "ProductWebsiteMapId", "dbo.ProductsWebsitesMaps");
            DropIndex("dbo.WebsiteSliders", new[] { "SiteID" });
            DropIndex("dbo.WebsiteContentPages", new[] { "SiteID" });
            DropIndex("dbo.WebsiteNavigations", new[] { "ParentId" });
            DropIndex("dbo.WebsiteNavigations", new[] { "SiteID" });
            DropIndex("dbo.ProductsNavigationMaps", new[] { "NavigationId" });
            DropIndex("dbo.ProductsNavigationMaps", new[] { "ProductWebsiteMapId" });
            DropColumn("dbo.ProductsWebsitesMaps", "IsActive");
            DropColumn("dbo.ProductsWebsitesMaps", "SortOrder");
            DropColumn("dbo.TenantWebsites", "IsActive");
            DropColumn("dbo.TenantWebsites", "FooterText");
            DropColumn("dbo.TenantWebsites", "InstaGramUrl");
            DropColumn("dbo.TenantWebsites", "YoutubeUrl");
            DropColumn("dbo.TenantWebsites", "LinkedInUrl");
            DropColumn("dbo.TenantWebsites", "TwitterUrl");
            DropColumn("dbo.TenantWebsites", "FacebookUrl");
            DropColumn("dbo.TenantWebsites", "Logo");
            DropColumn("dbo.TenantWebsites", "HostName");
            DropTable("dbo.WebsiteSliders");
            DropTable("dbo.WebsiteContentPages");
            DropTable("dbo.WebsiteNavigations");
            DropTable("dbo.ProductsNavigationMaps");
            RenameIndex(table: "dbo.TenantWebsites", name: "IX_DefaultWarehouseId", newName: "IX_WarehouseId");
            RenameColumn(table: "dbo.TenantWebsites", name: "DefaultWarehouseId", newName: "WarehouseId");
        }
    }
}
