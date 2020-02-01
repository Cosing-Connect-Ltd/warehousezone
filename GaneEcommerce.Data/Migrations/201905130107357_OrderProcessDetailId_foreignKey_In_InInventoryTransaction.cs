namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderProcessDetailId_foreignKey_In_InInventoryTransaction : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.InventoryTransactions", "OrderProcessDetailId");
            AddForeignKey("dbo.InventoryTransactions", "OrderProcessDetailId", "dbo.OrderProcessDetails", "OrderProcessDetailID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryTransactions", "OrderProcessDetailId", "dbo.OrderProcessDetails");
            DropIndex("dbo.InventoryTransactions", new[] { "OrderProcessDetailId" });
        }
    }
}
