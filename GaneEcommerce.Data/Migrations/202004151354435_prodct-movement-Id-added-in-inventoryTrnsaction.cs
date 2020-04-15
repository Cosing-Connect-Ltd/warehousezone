namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prodctmovementIdaddedininventoryTrnsaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "ProductMovementId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryTransactions", "ProductMovementId");
        }
    }
}
