namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SiteIDaddedinProductAllownceAndGroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductAllowanceGroupMaps", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductAllowanceGroupMaps", new[] { "ProductId" });
            AddColumn("dbo.ProductAllowances", "SiteID", c => c.Int(nullable: false));
            AddColumn("dbo.ProductAllowanceGroups", "SiteID", c => c.Int(nullable: false));
            AddColumn("dbo.ProductAllowanceGroupMaps", "ProductwebsiteMapId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductAllowances", "SiteID");
            CreateIndex("dbo.ProductAllowanceGroups", "SiteID");
            CreateIndex("dbo.ProductAllowanceGroupMaps", "ProductwebsiteMapId");
            AddForeignKey("dbo.ProductAllowanceGroupMaps", "ProductwebsiteMapId", "dbo.ProductsWebsitesMaps", "Id");
            AddForeignKey("dbo.ProductAllowanceGroups", "SiteID", "dbo.TenantWebsites", "SiteID");
            AddForeignKey("dbo.ProductAllowances", "SiteID", "dbo.TenantWebsites", "SiteID");
            DropColumn("dbo.ProductAllowanceGroupMaps", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductAllowanceGroupMaps", "ProductId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ProductAllowances", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.ProductAllowanceGroups", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.ProductAllowanceGroupMaps", "ProductwebsiteMapId", "dbo.ProductsWebsitesMaps");
            DropIndex("dbo.ProductAllowanceGroupMaps", new[] { "ProductwebsiteMapId" });
            DropIndex("dbo.ProductAllowanceGroups", new[] { "SiteID" });
            DropIndex("dbo.ProductAllowances", new[] { "SiteID" });
            DropColumn("dbo.ProductAllowanceGroupMaps", "ProductwebsiteMapId");
            DropColumn("dbo.ProductAllowanceGroups", "SiteID");
            DropColumn("dbo.ProductAllowances", "SiteID");
            CreateIndex("dbo.ProductAllowanceGroupMaps", "ProductId");
            AddForeignKey("dbo.ProductAllowanceGroupMaps", "ProductId", "dbo.ProductMaster", "ProductId");
        }
    }
}
