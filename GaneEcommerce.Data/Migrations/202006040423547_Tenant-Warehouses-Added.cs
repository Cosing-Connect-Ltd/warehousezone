namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantWarehousesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteWarehouses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
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
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.WarehouseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteWarehouses", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteWarehouses", "WarehouseId", "dbo.TenantLocations");
            DropIndex("dbo.WebsiteWarehouses", new[] { "WarehouseId" });
            DropIndex("dbo.WebsiteWarehouses", new[] { "SiteID" });
            DropTable("dbo.WebsiteWarehouses");
        }
    }
}
