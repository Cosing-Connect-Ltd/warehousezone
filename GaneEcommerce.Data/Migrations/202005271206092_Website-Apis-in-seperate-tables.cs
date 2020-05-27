namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebsiteApisinseperatetables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "BaseFilePath", c => c.String(nullable: false));
            AddColumn("dbo.GlobalApis", "DefaultWarehouseId", c => c.Int(nullable: false));
            AddColumn("dbo.GlobalApis", "SiteID", c => c.Int());
            AddColumn("dbo.GlobalApis", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.GlobalApis", "DateUpdated", c => c.DateTime());
            AddColumn("dbo.GlobalApis", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.TenantWebsites", "HostName", c => c.String(nullable: false));
            AlterColumn("dbo.GlobalApis", "CreatedBy", c => c.Int());
            AlterColumn("dbo.GlobalApis", "TenantId", c => c.Int(nullable: false));
            CreateIndex("dbo.GlobalApis", "DefaultWarehouseId");
            CreateIndex("dbo.GlobalApis", "SiteID");
            AddForeignKey("dbo.GlobalApis", "SiteID", "dbo.TenantWebsites", "SiteID");
            AddForeignKey("dbo.GlobalApis", "DefaultWarehouseId", "dbo.TenantLocations", "WarehouseId");
            DropColumn("dbo.TenantWebsites", "SiteType");
            DropColumn("dbo.TenantWebsites", "SiteApiUrl");
            DropColumn("dbo.TenantWebsites", "ApiToken");
            DropColumn("dbo.GlobalApis", "CreatedDate");
            DropColumn("dbo.GlobalApis", "UpdatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GlobalApis", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.GlobalApis", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TenantWebsites", "ApiToken", c => c.String());
            AddColumn("dbo.TenantWebsites", "SiteApiUrl", c => c.String());
            AddColumn("dbo.TenantWebsites", "SiteType", c => c.Int(nullable: false));
            DropForeignKey("dbo.GlobalApis", "DefaultWarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.GlobalApis", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.GlobalApis", new[] { "SiteID" });
            DropIndex("dbo.GlobalApis", new[] { "DefaultWarehouseId" });
            AlterColumn("dbo.GlobalApis", "TenantId", c => c.Int());
            AlterColumn("dbo.GlobalApis", "CreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.TenantWebsites", "HostName", c => c.String());
            DropColumn("dbo.GlobalApis", "IsDeleted");
            DropColumn("dbo.GlobalApis", "DateUpdated");
            DropColumn("dbo.GlobalApis", "DateCreated");
            DropColumn("dbo.GlobalApis", "SiteID");
            DropColumn("dbo.GlobalApis", "DefaultWarehouseId");
            DropColumn("dbo.TenantWebsites", "BaseFilePath");
        }
    }
}
