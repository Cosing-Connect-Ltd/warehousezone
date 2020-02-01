namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedMarketJobEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MarketJobs", "MarketJobStatusId", "dbo.MarketJobStatus");
            DropForeignKey("dbo.ResourceJobAllocation", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.ResourceJobAllocation", "ResourceJobStatusId", "dbo.ResourceJobStatus");
            DropIndex("dbo.MarketJobs", new[] { "MarketJobStatusId" });
            DropIndex("dbo.ResourceJobAllocation", new[] { "ResourceId" });
            DropIndex("dbo.ResourceJobAllocation", new[] { "ResourceJobStatusId" });
            CreateTable(
                "dbo.MarketJobAllocation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        ScheduledDate = c.DateTime(),
                        DeclinedReason = c.String(),
                        DeclinedDate = c.DateTime(),
                        CompletedReason = c.String(),
                        CompletedDate = c.DateTime(),
                        DeviceSerial = c.String(),
                        MarketJobId = c.Int(nullable: false),
                        MarketJobStatusId = c.Int(nullable: false),
                        AcceptedGeoLocation = c.Geography(),
                        CancelledGeoLocation = c.Geography(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MarketJobs", t => t.MarketJobId)
                .ForeignKey("dbo.MarketJobStatus", t => t.MarketJobStatusId)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .Index(t => t.ResourceId)
                .Index(t => t.MarketJobId)
                .Index(t => t.MarketJobStatusId);
            
            AddColumn("dbo.MarketJobs", "LatestJobStatusId", c => c.Int());
            AddColumn("dbo.MarketJobs", "LatestJobAllocationId", c => c.Int());
            DropColumn("dbo.MarketJobs", "MarketJobStatusId");
            DropColumn("dbo.MarketJobs", "DateCancelled");
            DropColumn("dbo.MarketJobs", "CancelledReason");
            DropColumn("dbo.MarketJobs", "DateDeclined");
            DropColumn("dbo.MarketJobs", "DeclinedReason");
            DropColumn("dbo.MarketJobs", "DeviceIdentifier");
            DropColumn("dbo.MarketJobs", "DeviceUsername");
            DropTable("dbo.ResourceJobAllocation");
            DropTable("dbo.ResourceJobStatus");
            DropTable("dbo.ResourceJob");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ResourceJob",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        RequiredDate = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceJobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceJobAllocation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        ScheduledDate = c.DateTime(),
                        DeclinedComment = c.String(),
                        DeclinedDate = c.DateTime(),
                        CompletedComment = c.String(),
                        CompletedDate = c.DateTime(),
                        ResourceJobStatusId = c.Int(nullable: false),
                        AcceptedGeoLocation = c.Geography(),
                        CancelledGeoLocation = c.Geography(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MarketJobs", "DeviceUsername", c => c.String());
            AddColumn("dbo.MarketJobs", "DeviceIdentifier", c => c.String());
            AddColumn("dbo.MarketJobs", "DeclinedReason", c => c.String());
            AddColumn("dbo.MarketJobs", "DateDeclined", c => c.DateTime());
            AddColumn("dbo.MarketJobs", "CancelledReason", c => c.String());
            AddColumn("dbo.MarketJobs", "DateCancelled", c => c.DateTime());
            AddColumn("dbo.MarketJobs", "MarketJobStatusId", c => c.Int(nullable: false));
            DropForeignKey("dbo.MarketJobAllocation", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.MarketJobAllocation", "MarketJobStatusId", "dbo.MarketJobStatus");
            DropForeignKey("dbo.MarketJobAllocation", "MarketJobId", "dbo.MarketJobs");
            DropIndex("dbo.MarketJobAllocation", new[] { "MarketJobStatusId" });
            DropIndex("dbo.MarketJobAllocation", new[] { "MarketJobId" });
            DropIndex("dbo.MarketJobAllocation", new[] { "ResourceId" });
            DropColumn("dbo.MarketJobs", "LatestJobAllocationId");
            DropColumn("dbo.MarketJobs", "LatestJobStatusId");
            DropTable("dbo.MarketJobAllocation");
            CreateIndex("dbo.ResourceJobAllocation", "ResourceJobStatusId");
            CreateIndex("dbo.ResourceJobAllocation", "ResourceId");
            CreateIndex("dbo.MarketJobs", "MarketJobStatusId");
            AddForeignKey("dbo.ResourceJobAllocation", "ResourceJobStatusId", "dbo.ResourceJobStatus", "Id");
            AddForeignKey("dbo.ResourceJobAllocation", "ResourceId", "dbo.Resources", "ResourceId");
            AddForeignKey("dbo.MarketJobs", "MarketJobStatusId", "dbo.MarketJobStatus", "Id");
        }
    }
}
