namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalCommandModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TerminalCommandsQueues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommandId = c.Int(nullable: false),
                        ExecutionDate = c.DateTime(nullable: false),
                        sent = c.Boolean(nullable: false),
                        SentDate = c.DateTime(),
                        result = c.Boolean(nullable: false),
                        ResultDate = c.DateTime(),
                        TerminalId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .ForeignKey("dbo.TerminalCommands", t => t.CommandId)
                .Index(t => t.CommandId)
                .Index(t => t.TerminalId);
            
            CreateTable(
                "dbo.TerminalCommands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommandIdentifier = c.String(),
                        CommandString = c.String(),
                        CommandDescription = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerminalCommandsQueues", "CommandId", "dbo.TerminalCommands");
            DropForeignKey("dbo.TerminalCommandsQueues", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.TerminalCommandsQueues", new[] { "TerminalId" });
            DropIndex("dbo.TerminalCommandsQueues", new[] { "CommandId" });
            DropTable("dbo.TerminalCommands");
            DropTable("dbo.TerminalCommandsQueues");
        }
    }
}
