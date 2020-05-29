namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryTransactionTypesTableRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryTransactions", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.OrderProcesses", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.Orders", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropIndex("dbo.Orders", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.InventoryTransactions", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.OrderProcesses", new[] { "InventoryTransactionTypeId" });
            DropTable("dbo.InventoryTransactionTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InventoryTransactionTypes",
                c => new
                    {
                        InventoryTransactionTypeId = c.Int(nullable: false, identity: true),
                        OrderType = c.String(nullable: false),
                        InventoryTransactionTypeName = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.InventoryTransactionTypeId);
            
            CreateIndex("dbo.OrderProcesses", "InventoryTransactionTypeId");
            CreateIndex("dbo.InventoryTransactions", "InventoryTransactionTypeId");
            CreateIndex("dbo.Orders", "InventoryTransactionTypeId");
            AddForeignKey("dbo.Orders", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes", "InventoryTransactionTypeId");
            AddForeignKey("dbo.OrderProcesses", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes", "InventoryTransactionTypeId");
            AddForeignKey("dbo.InventoryTransactions", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes", "InventoryTransactionTypeId");
        }
    }
}
