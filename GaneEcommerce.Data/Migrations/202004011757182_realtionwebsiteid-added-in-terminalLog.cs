namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realtionwebsiteidaddedinterminalLog : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TenantWebsitesSyncLogs", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.TenantWebsitesSyncLogs", new[] { "SiteID" });
            CreateIndex("dbo.TerminalsLogs", "SiteID");
            AddForeignKey("dbo.TerminalsLogs", "SiteID", "dbo.TenantWebsites", "SiteID");
            DropTable("dbo.TenantWebsitesSyncLogs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TenantWebsitesSyncLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestTime = c.DateTime(nullable: false),
                        RequestUrl = c.String(),
                        ErrorCode = c.Int(nullable: false),
                        Synced = c.Boolean(nullable: false),
                        ResultCount = c.Int(nullable: false),
                        ResponseTime = c.DateTime(nullable: false),
                        RequestSentDate = c.DateTime(nullable: false),
                        request = c.String(),
                        LogType = c.Int(nullable: false),
                        SiteID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TerminalsLogs", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.TerminalsLogs", new[] { "SiteID" });
            CreateIndex("dbo.TenantWebsitesSyncLogs", "SiteID");
            AddForeignKey("dbo.TenantWebsitesSyncLogs", "SiteID", "dbo.TenantWebsites", "SiteID");
        }
    }
}
