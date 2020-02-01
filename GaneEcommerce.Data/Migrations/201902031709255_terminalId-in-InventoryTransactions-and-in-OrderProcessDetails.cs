namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class terminalIdinInventoryTransactionsandinOrderProcessDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryTransactions", "TerminalId", c => c.Int());
            AddColumn("dbo.OrderProcessDetails", "TerminalId", c => c.Int());
            CreateIndex("dbo.InventoryTransactions", "TerminalId");
            CreateIndex("dbo.OrderProcessDetails", "TerminalId");
            AddForeignKey("dbo.OrderProcessDetails", "TerminalId", "dbo.Terminals", "TerminalId");
            AddForeignKey("dbo.InventoryTransactions", "TerminalId", "dbo.Terminals", "TerminalId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryTransactions", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.OrderProcessDetails", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.OrderProcessDetails", new[] { "TerminalId" });
            DropIndex("dbo.InventoryTransactions", new[] { "TerminalId" });
            DropColumn("dbo.OrderProcessDetails", "TerminalId");
            DropColumn("dbo.InventoryTransactions", "TerminalId");
        }
    }
}
