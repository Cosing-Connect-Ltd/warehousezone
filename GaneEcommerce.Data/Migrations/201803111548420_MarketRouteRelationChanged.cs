namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MarketRouteRelationChanged : DbMigration
    {
        public override void Up()
        {   
            DropForeignKey("dbo.MarketRouteCustomers", "MarketRouteId", "dbo.MarketRoutes");
            RenameTable(name: "dbo.MarketRouteCustomers", newName: "MarketCustomers");
            DropForeignKey("dbo.MarketRoutes", "LastVehicleId", "dbo.MarketVehicles");   
            DropForeignKey("dbo.MarketVehicles", "MarketRoute_Id", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketVehicles", "MarketId", "dbo.Markets");
            DropIndex("dbo.MarketVehicles", new[] { "MarketId" });
            DropIndex("dbo.MarketVehicles", new[] { "MarketRoute_Id" });
            DropIndex("dbo.MarketRoutes", new[] { "LastVehicleId" });
            DropIndex("dbo.MarketRoutes", new[] { "MarketId" });
            DropIndex("dbo.MarketCustomers", new[] { "MarketRouteId" });
            AddColumn("dbo.Markets", "MarketId", c => c.Int());
            DropForeignKey("dbo.MarketRoutes", "MarketId", "dbo.Markets");
            RenameColumn(table: "dbo.Markets", name: "MarketId", newName: "RouteId");
            CreateTable(
                "dbo.ProductMarketStockLevel",
                c => new
                    {
                        ProductMarketStockLevelID = c.Int(nullable: false, identity: true),
                        ProductMasterID = c.Int(nullable: false),
                        MarketId = c.Int(nullable: false),
                        MinStockQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductMarketStockLevelID)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMasterID)
                .Index(t => t.ProductMasterID)
                .Index(t => t.MarketId);
            
            AddColumn("dbo.MarketCustomers", "VisitFrequency", c => c.Int(nullable: false));
            AddColumn("dbo.MarketCustomers", "MarketId", c => c.Int(nullable: false));
            CreateIndex("dbo.Markets", "RouteId");
            CreateIndex("dbo.MarketCustomers", "MarketId");
            AddForeignKey("dbo.MarketCustomers", "MarketId", "dbo.Markets", "Id");
            DropColumn("dbo.MarketVehicles", "MarketId");
            DropColumn("dbo.MarketVehicles", "MarketRoute_Id");
            DropColumn("dbo.MarketRoutes", "IsDefaultRoute");
            DropColumn("dbo.MarketRoutes", "LastVehicleId");
            DropColumn("dbo.MarketRoutes", "MarketId");
            DropColumn("dbo.MarketCustomers", "MarketRouteId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MarketCustomers", "MarketRouteId", c => c.Int(nullable: false));
            AddColumn("dbo.MarketRoutes", "MarketId", c => c.Int());
            AddColumn("dbo.MarketRoutes", "LastVehicleId", c => c.Int());
            AddColumn("dbo.MarketRoutes", "IsDefaultRoute", c => c.Boolean(nullable: false));
            AddColumn("dbo.MarketVehicles", "MarketRoute_Id", c => c.Int());
            AddColumn("dbo.MarketVehicles", "MarketId", c => c.Int());
            DropForeignKey("dbo.ProductMarketStockLevel", "ProductMasterID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductMarketStockLevel", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketCustomers", "MarketId", "dbo.Markets");
            DropIndex("dbo.ProductMarketStockLevel", new[] { "MarketId" });
            DropIndex("dbo.ProductMarketStockLevel", new[] { "ProductMasterID" });
            DropIndex("dbo.MarketCustomers", new[] { "MarketId" });
            DropIndex("dbo.Markets", new[] { "RouteId" });
            DropColumn("dbo.MarketCustomers", "MarketId");
            DropColumn("dbo.MarketCustomers", "VisitFrequency");
            DropTable("dbo.ProductMarketStockLevel");
            RenameColumn(table: "dbo.Markets", name: "RouteId", newName: "MarketId");
            AddForeignKey("dbo.MarketRoutes", "MarketId", "dbo.Markets");
            DropColumn("dbo.Markets", "MarketId");
            CreateIndex("dbo.MarketCustomers", "MarketRouteId");
            CreateIndex("dbo.MarketRoutes", "MarketId");
            CreateIndex("dbo.MarketRoutes", "LastVehicleId");
            CreateIndex("dbo.MarketVehicles", "MarketRoute_Id");
            CreateIndex("dbo.MarketVehicles", "MarketId");
            AddForeignKey("dbo.MarketVehicles", "MarketId", "dbo.Markets", "Id");
            AddForeignKey("dbo.MarketVehicles", "MarketRoute_Id", "dbo.MarketRoutes", "Id");
            AddForeignKey("dbo.MarketRoutes", "LastVehicleId", "dbo.MarketVehicles", "Id");
            RenameTable(name: "dbo.MarketCustomers", newName: "MarketRouteCustomers");
            AddForeignKey("dbo.MarketRouteCustomers", "MarketRouteId", "dbo.MarketRoutes", "Id");    
        }
    }
}
