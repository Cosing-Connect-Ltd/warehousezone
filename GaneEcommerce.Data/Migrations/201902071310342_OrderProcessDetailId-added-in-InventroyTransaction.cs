namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderProcessDetailIdaddedinInventroyTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "OrderProcessDetailId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "OrderProcessDetailId");
        }
    }
}
