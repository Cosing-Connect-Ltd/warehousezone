namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventorystocksfrompersistableentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InventoryStocks", "CreatedBy", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InventoryStocks", "CreatedBy", c => c.Int(nullable: false));
        }
    }
}
