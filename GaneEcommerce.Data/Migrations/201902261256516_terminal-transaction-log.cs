namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class terminaltransactionlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TerminalsTransactionsLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TerminalId = c.Int(nullable: false),
                        TransactionLogReference = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.TerminalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerminalsTransactionsLogs", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.TerminalsTransactionsLogs", new[] { "TerminalId" });
            DropTable("dbo.TerminalsTransactionsLogs");
        }
    }
}
