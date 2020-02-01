namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryTransactionAuthUserforeignkeyadded : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.InventoryTransactions", "CreatedBy");
            AddForeignKey("dbo.InventoryTransactions", "CreatedBy", "dbo.AuthUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryTransactions", "CreatedBy", "dbo.AuthUsers");
            DropIndex("dbo.InventoryTransactions", new[] { "CreatedBy" });
        }
    }
}
