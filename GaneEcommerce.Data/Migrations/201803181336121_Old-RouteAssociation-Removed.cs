namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OldRouteAssociationRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MarketRouteAssociations", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketRouteAssociations", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteAssociations", "TenantLocationID", "dbo.TenantLocations");
            DropForeignKey("dbo.MarketJobs", "MarketRouteAssociationId", "dbo.MarketRouteAssociations");
            DropIndex("dbo.MarketJobs", new[] { "MarketRouteAssociationId" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "MarketId" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "MarketRouteId" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "TenantLocationID" });
            AddColumn("dbo.MarketJobs", "MarketRouteId", c => c.Int());
            AddColumn("dbo.MarketJobs", "MarketId", c => c.Int());
            CreateIndex("dbo.MarketJobs", "MarketRouteId");
            CreateIndex("dbo.MarketJobs", "MarketId");
            AddForeignKey("dbo.MarketJobs", "MarketId", "dbo.Markets", "Id");
            AddForeignKey("dbo.MarketJobs", "MarketRouteId", "dbo.MarketRoutes", "Id");
            DropColumn("dbo.MarketJobs", "MarketRouteAssociationId");
            DropTable("dbo.MarketRouteAssociations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MarketRouteAssociations",
                c => new
                    {
                        MarketRouteAssociationId = c.Int(nullable: false, identity: true),
                        MarketId = c.Int(nullable: false),
                        MarketRouteId = c.Int(nullable: false),
                        TenantLocationID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.MarketRouteAssociationId);
            
            AddColumn("dbo.MarketJobs", "MarketRouteAssociationId", c => c.Int());
            DropForeignKey("dbo.MarketJobs", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketJobs", "MarketId", "dbo.Markets");
            DropIndex("dbo.MarketJobs", new[] { "MarketId" });
            DropIndex("dbo.MarketJobs", new[] { "MarketRouteId" });
            DropColumn("dbo.MarketJobs", "MarketId");
            DropColumn("dbo.MarketJobs", "MarketRouteId");
            CreateIndex("dbo.MarketRouteAssociations", "TenantLocationID");
            CreateIndex("dbo.MarketRouteAssociations", "MarketRouteId");
            CreateIndex("dbo.MarketRouteAssociations", "MarketId");
            CreateIndex("dbo.MarketJobs", "MarketRouteAssociationId");
            AddForeignKey("dbo.MarketJobs", "MarketRouteAssociationId", "dbo.MarketRouteAssociations", "MarketRouteAssociationId");
            AddForeignKey("dbo.MarketRouteAssociations", "TenantLocationID", "dbo.TenantLocations", "WarehouseId");
            AddForeignKey("dbo.MarketRouteAssociations", "MarketRouteId", "dbo.MarketRoutes", "Id");
            AddForeignKey("dbo.MarketRouteAssociations", "MarketId", "dbo.Markets", "Id");
        }
    }
}
