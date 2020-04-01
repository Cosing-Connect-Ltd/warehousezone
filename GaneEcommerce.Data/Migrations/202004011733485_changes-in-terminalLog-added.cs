namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesinterminalLogadded : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TerminalsLogs", new[] { "TerminalId" });
            AddColumn("dbo.TerminalsLogs", "SiteID", c => c.Int());
            AlterColumn("dbo.TerminalsLogs", "TerminalId", c => c.Int());
            CreateIndex("dbo.TerminalsLogs", "TerminalId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TerminalsLogs", new[] { "TerminalId" });
            AlterColumn("dbo.TerminalsLogs", "TerminalId", c => c.Int(nullable: false));
            DropColumn("dbo.TerminalsLogs", "SiteID");
            CreateIndex("dbo.TerminalsLogs", "TerminalId");
        }
    }
}
