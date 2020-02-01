namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalGeoLocationAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TerminalGeoLocations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Date = c.DateTime(nullable: false),
                        LoggedInUserId = c.Int(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthUsers", t => t.LoggedInUserId)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.TerminalId)
                .Index(t => t.LoggedInUserId);
            
            AddColumn("dbo.MarketJobAllocation", "Reason", c => c.String());
            AddColumn("dbo.MarketJobAllocation", "ActionDate", c => c.DateTime());
            AddColumn("dbo.MarketJobAllocation", "Latitude", c => c.Double());
            AddColumn("dbo.MarketJobAllocation", "Longitude", c => c.Double());
            AddColumn("dbo.MarketRouteProgresses", "Latitude", c => c.Double());
            AddColumn("dbo.MarketRouteProgresses", "Longitude", c => c.Double());
            DropColumn("dbo.MarketJobAllocation", "DeclinedReason");
            DropColumn("dbo.MarketJobAllocation", "DeclinedDate");
            DropColumn("dbo.MarketJobAllocation", "CompletedReason");
            DropColumn("dbo.MarketJobAllocation", "CompletedDate");
            DropColumn("dbo.MarketJobAllocation", "AcceptedGeoLocation");
            DropColumn("dbo.MarketJobAllocation", "CancelledGeoLocation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MarketJobAllocation", "CancelledGeoLocation", c => c.Geography());
            AddColumn("dbo.MarketJobAllocation", "AcceptedGeoLocation", c => c.Geography());
            AddColumn("dbo.MarketJobAllocation", "CompletedDate", c => c.DateTime());
            AddColumn("dbo.MarketJobAllocation", "CompletedReason", c => c.String());
            AddColumn("dbo.MarketJobAllocation", "DeclinedDate", c => c.DateTime());
            AddColumn("dbo.MarketJobAllocation", "DeclinedReason", c => c.String());
            DropForeignKey("dbo.TerminalGeoLocations", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.TerminalGeoLocations", "LoggedInUserId", "dbo.AuthUsers");
            DropIndex("dbo.TerminalGeoLocations", new[] { "LoggedInUserId" });
            DropIndex("dbo.TerminalGeoLocations", new[] { "TerminalId" });
            DropColumn("dbo.MarketRouteProgresses", "Longitude");
            DropColumn("dbo.MarketRouteProgresses", "Latitude");
            DropColumn("dbo.MarketJobAllocation", "Longitude");
            DropColumn("dbo.MarketJobAllocation", "Latitude");
            DropColumn("dbo.MarketJobAllocation", "ActionDate");
            DropColumn("dbo.MarketJobAllocation", "Reason");
            DropTable("dbo.TerminalGeoLocations");
        }
    }
}
