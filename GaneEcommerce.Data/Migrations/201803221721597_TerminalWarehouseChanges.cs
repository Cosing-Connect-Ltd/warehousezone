namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalWarehouseChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Terminals", "WarehouseId", "dbo.TenantLocations");
            CreateTable(
                "dbo.MarketRouteProgresses",
                c => new
                    {
                        RouteProgressId = c.Guid(nullable: false),
                        MarketId = c.Int(nullable: false),
                        MarketRouteId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                        OrderId = c.Int(),
                        Comment = c.String(),
                        SaleMade = c.Boolean(),
                        Timestamp = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.RouteProgressId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.MarketRoutes", t => t.MarketRouteId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.MarketId)
                .Index(t => t.MarketRouteId)
                .Index(t => t.AccountId)
                .Index(t => t.OrderId);
            
            AddColumn("dbo.TenantLocations", "SalesTerminalId", c => c.Int());
            AddColumn("dbo.TenantLocations", "SalesManUserId", c => c.Int());
            AddColumn("dbo.Terminals", "TenantLocations_WarehouseId", c => c.Int());
            CreateIndex("dbo.TenantLocations", "SalesTerminalId");
            CreateIndex("dbo.TenantLocations", "SalesManUserId");
            CreateIndex("dbo.Terminals", "TenantLocations_WarehouseId");
            AddForeignKey("dbo.TenantLocations", "SalesManUserId", "dbo.AuthUsers", "UserId");
            AddForeignKey("dbo.TenantLocations", "SalesTerminalId", "dbo.Terminals", "TerminalId");
            AddForeignKey("dbo.Terminals", "TenantLocations_WarehouseId", "dbo.TenantLocations", "WarehouseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Terminals", "TenantLocations_WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.MarketRouteProgresses", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.MarketRouteProgresses", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteProgresses", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketRouteProgresses", "AccountId", "dbo.Account");
            DropForeignKey("dbo.TenantLocations", "SalesTerminalId", "dbo.Terminals");
            DropForeignKey("dbo.TenantLocations", "SalesManUserId", "dbo.AuthUsers");
            DropIndex("dbo.MarketRouteProgresses", new[] { "OrderId" });
            DropIndex("dbo.MarketRouteProgresses", new[] { "AccountId" });
            DropIndex("dbo.MarketRouteProgresses", new[] { "MarketRouteId" });
            DropIndex("dbo.MarketRouteProgresses", new[] { "MarketId" });
            DropIndex("dbo.Terminals", new[] { "TenantLocations_WarehouseId" });
            DropIndex("dbo.TenantLocations", new[] { "SalesManUserId" });
            DropIndex("dbo.TenantLocations", new[] { "SalesTerminalId" });
            DropColumn("dbo.Terminals", "TenantLocations_WarehouseId");
            DropColumn("dbo.TenantLocations", "SalesManUserId");
            DropColumn("dbo.TenantLocations", "SalesTerminalId");
            DropTable("dbo.MarketRouteProgresses");
            AddForeignKey("dbo.Terminals", "WarehouseId", "dbo.TenantLocations", "WarehouseId");
        }
    }
}
