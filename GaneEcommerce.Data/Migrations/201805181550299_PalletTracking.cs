namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PalletTracking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PalletTrackings",
                c => new
                    {
                        PalletTrackingId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PalletSerial = c.String(),
                        ExpiryDate = c.DateTime(),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BatchNo = c.String(),
                        Status = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.PalletTrackingId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.TenantId)
                .Index(t => t.WarehouseId);
            
            AddColumn("dbo.InventoryTransactions", "PalletTrackingId", c => c.Int());
            CreateIndex("dbo.InventoryTransactions", "PalletTrackingId");
            AddForeignKey("dbo.InventoryTransactions", "PalletTrackingId", "dbo.PalletTrackings", "PalletTrackingId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PalletTrackings", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.PalletTrackings", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.PalletTrackings", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.InventoryTransactions", "PalletTrackingId", "dbo.PalletTrackings");
            DropIndex("dbo.PalletTrackings", new[] { "WarehouseId" });
            DropIndex("dbo.PalletTrackings", new[] { "TenantId" });
            DropIndex("dbo.PalletTrackings", new[] { "ProductId" });
            DropIndex("dbo.InventoryTransactions", new[] { "PalletTrackingId" });
            DropColumn("dbo.InventoryTransactions", "PalletTrackingId");
            DropTable("dbo.PalletTrackings");
        }
    }
}
