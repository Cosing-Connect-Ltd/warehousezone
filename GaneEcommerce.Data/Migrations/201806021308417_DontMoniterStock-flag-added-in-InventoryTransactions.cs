namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DontMoniterStockflagaddedinInventoryTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "DontMonitorStock", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "DontMonitorStock");
        }
    }
}
