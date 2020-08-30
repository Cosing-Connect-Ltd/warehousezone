namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiIdrelationaddedinterminallogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TerminalsLogs", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.TerminalsLogs", new[] { "SiteID" });
            AddColumn("dbo.TerminalsLogs", "ApiId", c => c.Int());
            CreateIndex("dbo.TerminalsLogs", "ApiId");
            AddForeignKey("dbo.TerminalsLogs", "ApiId", "dbo.ApiCredentials", "Id");
            DropColumn("dbo.TerminalsLogs", "SiteID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TerminalsLogs", "SiteID", c => c.Int());
            DropForeignKey("dbo.TerminalsLogs", "ApiId", "dbo.ApiCredentials");
            DropIndex("dbo.TerminalsLogs", new[] { "ApiId" });
            DropColumn("dbo.TerminalsLogs", "ApiId");
            CreateIndex("dbo.TerminalsLogs", "SiteID");
            AddForeignKey("dbo.TerminalsLogs", "SiteID", "dbo.TenantWebsites", "SiteID");
        }
    }
}
