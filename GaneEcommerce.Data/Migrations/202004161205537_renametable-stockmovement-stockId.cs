namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renametablestockmovementstockId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockMovements",
                c => new
                    {
                        StockMovementId = c.Guid(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.StockMovementId);
            
            AddColumn("dbo.InventoryTransactions", "StockMovementId", c => c.Guid());
            DropColumn("dbo.InventoryTransactions", "ProductMovementId");
            DropTable("dbo.ProductMovements");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductMovements",
                c => new
                    {
                        ProductMovementId = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductMovementId);
            
            AddColumn("dbo.InventoryTransactions", "ProductMovementId", c => c.Guid());
            DropColumn("dbo.InventoryTransactions", "StockMovementId");
            DropTable("dbo.StockMovements");
        }
    }
}
