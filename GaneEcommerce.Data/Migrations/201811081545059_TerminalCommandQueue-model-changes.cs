namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalCommandQueuemodelchanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TerminalCommandsQueues", new[] { "TerminalId" });
            AddColumn("dbo.TerminalCommandsQueues", "resultString", c => c.String());
            AlterColumn("dbo.TerminalCommandsQueues", "TerminalId", c => c.Int(nullable: false));
            CreateIndex("dbo.TerminalCommandsQueues", "TerminalId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TerminalCommandsQueues", new[] { "TerminalId" });
            AlterColumn("dbo.TerminalCommandsQueues", "TerminalId", c => c.Int());
            DropColumn("dbo.TerminalCommandsQueues", "resultString");
            CreateIndex("dbo.TerminalCommandsQueues", "TerminalId");
        }
    }
}
