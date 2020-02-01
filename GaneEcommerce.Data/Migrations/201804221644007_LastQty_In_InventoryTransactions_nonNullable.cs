namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastQty_In_InventoryTransactions_nonNullable : DbMigration
    {
        public override void Up()
        {
            Sql("Update dbo.InventoryTransactions SET LastQty = " + 0M);
            AlterColumn("dbo.InventoryTransactions", "LastQty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InventoryTransactions", "LastQty", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
