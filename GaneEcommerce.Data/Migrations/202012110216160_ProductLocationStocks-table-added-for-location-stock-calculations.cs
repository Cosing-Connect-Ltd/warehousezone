namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductLocationStockstableaddedforlocationstockcalculations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductLocationStocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.LocationId)
                .Index(t => t.WarehouseId);
            
            AlterColumn("dbo.ProductLocations", "CreatedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductLocationStocks", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.ProductLocationStocks", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductLocationStocks", "LocationId", "dbo.Locations");
            DropIndex("dbo.ProductLocationStocks", new[] { "WarehouseId" });
            DropIndex("dbo.ProductLocationStocks", new[] { "LocationId" });
            DropIndex("dbo.ProductLocationStocks", new[] { "ProductId" });
            AlterColumn("dbo.ProductLocations", "CreatedBy", c => c.Int(nullable: false));
            DropTable("dbo.ProductLocationStocks");
        }
    }
}
