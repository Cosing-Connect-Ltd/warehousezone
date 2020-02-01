namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RouteSchedulerResourcefilednamechange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MarketRouteSchedules", "WarehouseIDs", c => c.String());
            DropColumn("dbo.MarketRouteSchedules", "VehicleIDs");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MarketRouteSchedules", "VehicleIDs", c => c.String());
            DropColumn("dbo.MarketRouteSchedules", "WarehouseIDs");
        }
    }
}
