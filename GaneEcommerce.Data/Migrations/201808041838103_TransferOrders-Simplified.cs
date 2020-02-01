namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransferOrdersSimplified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "TransferTo_WarehouseId", "dbo.TenantLocations");
            DropIndex("dbo.Orders", new[] { "TransferTo_WarehouseId" });
            RenameColumn(table: "dbo.Orders", name: "TransferFrom_WarehouseId", newName: "TransferWarehouseId");
            RenameIndex(table: "dbo.Orders", name: "IX_TransferFrom_WarehouseId", newName: "IX_TransferWarehouseId");
            DropColumn("dbo.Orders", "TransferTo_WarehouseId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "TransferTo_WarehouseId", c => c.Int());
            RenameIndex(table: "dbo.Orders", name: "IX_TransferWarehouseId", newName: "IX_TransferFrom_WarehouseId");
            RenameColumn(table: "dbo.Orders", name: "TransferWarehouseId", newName: "TransferFrom_WarehouseId");
            CreateIndex("dbo.Orders", "TransferTo_WarehouseId");
            AddForeignKey("dbo.Orders", "TransferTo_WarehouseId", "dbo.TenantLocations", "WarehouseId");
        }
    }
}
