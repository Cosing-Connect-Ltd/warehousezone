namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Palletdispatchmodelchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pallets", "OrderProcessID", c => c.Int());
            AddColumn("dbo.PalletsDispatches", "DispatchStatus", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.PalletsDispatches", "OrderProcessID", c => c.Int());
            CreateIndex("dbo.Pallets", "OrderProcessID");
            CreateIndex("dbo.PalletsDispatches", "OrderProcessID");
            AddForeignKey("dbo.Pallets", "OrderProcessID", "dbo.OrderProcesses", "OrderProcessID");
            AddForeignKey("dbo.PalletsDispatches", "OrderProcessID", "dbo.OrderProcesses", "OrderProcessID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PalletsDispatches", "OrderProcessID", "dbo.OrderProcesses");
            DropForeignKey("dbo.Pallets", "OrderProcessID", "dbo.OrderProcesses");
            DropIndex("dbo.PalletsDispatches", new[] { "OrderProcessID" });
            DropIndex("dbo.Pallets", new[] { "OrderProcessID" });
            DropColumn("dbo.PalletsDispatches", "OrderProcessID");
            DropColumn("dbo.PalletsDispatches", "DispatchStatus");
            DropColumn("dbo.Pallets", "OrderProcessID");
        }
    }
}
