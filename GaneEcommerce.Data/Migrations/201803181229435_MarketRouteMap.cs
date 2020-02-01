namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MarketRouteMap : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Markets", "RouteId", "dbo.MarketRoutes");
            DropIndex("dbo.Markets", new[] { "RouteId" });
            CreateTable(
                "dbo.MarketRouteMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MarketId = c.Int(nullable: false),
                        MarketRouteId = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.MarketRoutes", t => t.MarketRouteId)
                .Index(t => t.MarketId)
                .Index(t => t.MarketRouteId);
            
            DropColumn("dbo.Markets", "RouteId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Markets", "RouteId", c => c.Int());
            DropForeignKey("dbo.MarketRouteMaps", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteMaps", "MarketId", "dbo.Markets");
            DropIndex("dbo.MarketRouteMaps", new[] { "MarketRouteId" });
            DropIndex("dbo.MarketRouteMaps", new[] { "MarketId" });
            DropTable("dbo.MarketRouteMaps");
            CreateIndex("dbo.Markets", "RouteId");
            AddForeignKey("dbo.Markets", "RouteId", "dbo.MarketRoutes", "Id");
        }
    }
}
