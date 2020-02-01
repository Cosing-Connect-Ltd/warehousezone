namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MarketSchedulerrelationChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MarketRouteSchedules", "MarketId", "dbo.Markets");
            DropIndex("dbo.MarketRouteSchedules", new[] { "MarketId" });
            AddColumn("dbo.MarketRouteSchedules", "WarehouseId", c => c.Int(nullable: false));
            AddColumn("dbo.MarketRouteSchedules", "RouteId", c => c.Int(nullable: false));
            CreateIndex("dbo.MarketRouteSchedules", "WarehouseId");
            CreateIndex("dbo.MarketRouteSchedules", "RouteId");
            AddForeignKey("dbo.MarketRouteSchedules", "RouteId", "dbo.MarketRoutes", "Id");
            AddForeignKey("dbo.MarketRouteSchedules", "WarehouseId", "dbo.TenantLocations", "WarehouseId");
            DropColumn("dbo.MarketRouteSchedules", "MarketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MarketRouteSchedules", "MarketId", c => c.Int(nullable: false));
            DropForeignKey("dbo.MarketRouteSchedules", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.MarketRouteSchedules", "RouteId", "dbo.MarketRoutes");
            DropIndex("dbo.MarketRouteSchedules", new[] { "RouteId" });
            DropIndex("dbo.MarketRouteSchedules", new[] { "WarehouseId" });
            DropColumn("dbo.MarketRouteSchedules", "RouteId");
            DropColumn("dbo.MarketRouteSchedules", "WarehouseId");
            CreateIndex("dbo.MarketRouteSchedules", "MarketId");
            AddForeignKey("dbo.MarketRouteSchedules", "MarketId", "dbo.Markets", "Id");
        }
    }
}
