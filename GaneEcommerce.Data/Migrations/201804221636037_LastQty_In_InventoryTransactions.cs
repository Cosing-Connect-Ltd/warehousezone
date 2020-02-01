namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastQty_In_InventoryTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "LastQty", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "LastQty");
        }
    }
}
