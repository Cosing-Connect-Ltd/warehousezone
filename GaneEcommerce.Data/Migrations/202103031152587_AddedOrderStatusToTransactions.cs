namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderStatusToTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "OrderStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "OrderStatus");
        }
    }
}
