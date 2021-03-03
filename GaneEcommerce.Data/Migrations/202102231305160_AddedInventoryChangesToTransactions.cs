namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInventoryChangesToTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "InStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.InventoryTransactions", "OnOrder", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.InventoryTransactions", "Allocated", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.InventoryTransactions", "Available", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "Available");
            DropColumn("dbo.InventoryTransactions", "Allocated");
            DropColumn("dbo.InventoryTransactions", "OnOrder");
            DropColumn("dbo.InventoryTransactions", "InStock");
        }
    }
}
