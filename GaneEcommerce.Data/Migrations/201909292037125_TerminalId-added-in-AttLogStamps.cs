namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalIdaddedinAttLogStamps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttLogsStamps", "TnALogsStampType", c => c.Int(nullable: false));
            AddColumn("dbo.AttLogsStamps", "TerminalId", c => c.Int(nullable: false));
            CreateIndex("dbo.AttLogsStamps", "TerminalId");
            AddForeignKey("dbo.AttLogsStamps", "TerminalId", "dbo.Terminals", "TerminalId");
            DropTable("dbo.OperLogsStamps");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OperLogsStamps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SStamp = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.AttLogsStamps", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.AttLogsStamps", new[] { "TerminalId" });
            DropColumn("dbo.AttLogsStamps", "TerminalId");
            DropColumn("dbo.AttLogsStamps", "TnALogsStampType");
        }
    }
}
