namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettrackingmodelsadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetLogs",
                c => new
                    {
                        AssetLogId = c.Guid(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        AssetId = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        piAddress = c.String(),
                        uuid = c.String(),
                        major = c.Int(nullable: false),
                        minor = c.Int(nullable: false),
                        measuredPower = c.Short(nullable: false),
                        rssi = c.Short(nullable: false),
                        accuracy = c.Double(nullable: false),
                        proximity = c.String(),
                        address = c.String(),
                    })
                .PrimaryKey(t => t.AssetLogId)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.TerminalId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        AssetId = c.Int(nullable: false, identity: true),
                        AssetName = c.String(maxLength: 50),
                        AssetDescription = c.String(),
                        AssetCode = c.String(maxLength: 100),
                        AssetTag = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetLogs", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.AssetLogs", "AssetId", "dbo.Assets");
            DropIndex("dbo.AssetLogs", new[] { "AssetId" });
            DropIndex("dbo.AssetLogs", new[] { "TerminalId" });
            DropTable("dbo.Assets");
            DropTable("dbo.AssetLogs");
        }
    }
}
