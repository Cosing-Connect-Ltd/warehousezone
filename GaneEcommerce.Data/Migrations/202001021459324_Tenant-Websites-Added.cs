namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantWebsitesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TenantWebsites",
                c => new
                    {
                        SiteID = c.Int(nullable: false, identity: true),
                        SiteName = c.String(),
                        SiteDescription = c.String(),
                        SiteType = c.Int(nullable: false),
                        SiteApiUrl = c.String(),
                        ApiToken = c.String(),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.SiteID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.WarehouseId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantWebsitesSyncLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestTime = c.DateTime(nullable: false),
                        RequestUrl = c.String(),
                        ErrorCode = c.Int(nullable: false),
                        Synced = c.Boolean(nullable: false),
                        ResultCount = c.Int(nullable: false),
                        ResponseTime = c.DateTime(nullable: false),
                        request = c.String(),
                        SiteID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenantWebsitesSyncLogs", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.TenantWebsites", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.TenantWebsites", "TenantId", "dbo.Tenants");
            DropIndex("dbo.TenantWebsitesSyncLogs", new[] { "SiteID" });
            DropIndex("dbo.TenantWebsites", new[] { "TenantId" });
            DropIndex("dbo.TenantWebsites", new[] { "WarehouseId" });
            DropTable("dbo.TenantWebsitesSyncLogs");
            DropTable("dbo.TenantWebsites");
        }
    }
}
