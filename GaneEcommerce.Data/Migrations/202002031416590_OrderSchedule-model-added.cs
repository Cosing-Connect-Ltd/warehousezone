namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderSchedulemodeladded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Subject = c.String(),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        Label = c.Int(nullable: false),
                        Location = c.String(),
                        AllDay = c.Boolean(nullable: false),
                        EventType = c.Int(nullable: false),
                        RecurrenceInfo = c.String(),
                        ReminderInfo = c.String(),
                        MarketVehicleId = c.Int(),
                        ResourceIDs = c.String(),
                        PalletDispatchId = c.Int(),
                        TenentId = c.Int(),
                        IsCanceled = c.Boolean(nullable: false),
                        CancelReason = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PalletsDispatches", t => t.PalletDispatchId)
                .ForeignKey("dbo.MarketVehicles", t => t.MarketVehicleId)
                .Index(t => t.MarketVehicleId)
                .Index(t => t.PalletDispatchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderSchedules", "MarketVehicleId", "dbo.MarketVehicles");
            DropForeignKey("dbo.OrderSchedules", "PalletDispatchId", "dbo.PalletsDispatches");
            DropIndex("dbo.OrderSchedules", new[] { "PalletDispatchId" });
            DropIndex("dbo.OrderSchedules", new[] { "MarketVehicleId" });
            DropTable("dbo.OrderSchedules");
        }
    }
}
