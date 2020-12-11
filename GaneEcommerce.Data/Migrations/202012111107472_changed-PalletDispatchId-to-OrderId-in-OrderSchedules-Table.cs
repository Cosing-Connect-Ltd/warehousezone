namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedPalletDispatchIdtoOrderIdinOrderSchedulesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderSchedules", "PalletDispatchId", "dbo.PalletsDispatches");
            DropIndex("dbo.OrderSchedules", new[] { "PalletDispatchId" });
            AddColumn("dbo.OrderSchedules", "OrderId", c => c.Int());
            CreateIndex("dbo.OrderSchedules", "OrderId");
            AddForeignKey("dbo.OrderSchedules", "OrderId", "dbo.Orders", "OrderID");
            DropColumn("dbo.OrderSchedules", "PalletDispatchId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderSchedules", "PalletDispatchId", c => c.Int());
            DropForeignKey("dbo.OrderSchedules", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderSchedules", new[] { "OrderId" });
            DropColumn("dbo.OrderSchedules", "OrderId");
            CreateIndex("dbo.OrderSchedules", "PalletDispatchId");
            AddForeignKey("dbo.OrderSchedules", "PalletDispatchId", "dbo.PalletsDispatches", "PalletsDispatchID");
        }
    }
}
