namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockTakePalletsSnapshotadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockTakePalletsSnapshots",
                c => new
                    {
                        StockTakePalletSnapshotId = c.Int(nullable: false, identity: true),
                        StockTakeSnapshotId = c.Int(nullable: false),
                        StockTakeId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PalletTrackingId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockTakePalletSnapshotId)
                .ForeignKey("dbo.PalletTrackings", t => t.PalletTrackingId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.StockTakes", t => t.StockTakeId)
                .ForeignKey("dbo.StockTakeSnapshots", t => t.StockTakeSnapshotId)
                .Index(t => t.StockTakeSnapshotId)
                .Index(t => t.StockTakeId)
                .Index(t => t.ProductId)
                .Index(t => t.PalletTrackingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockTakePalletsSnapshots", "StockTakeSnapshotId", "dbo.StockTakeSnapshots");
            DropForeignKey("dbo.StockTakePalletsSnapshots", "StockTakeId", "dbo.StockTakes");
            DropForeignKey("dbo.StockTakePalletsSnapshots", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.StockTakePalletsSnapshots", "PalletTrackingId", "dbo.PalletTrackings");
            DropIndex("dbo.StockTakePalletsSnapshots", new[] { "PalletTrackingId" });
            DropIndex("dbo.StockTakePalletsSnapshots", new[] { "ProductId" });
            DropIndex("dbo.StockTakePalletsSnapshots", new[] { "StockTakeId" });
            DropIndex("dbo.StockTakePalletsSnapshots", new[] { "StockTakeSnapshotId" });
            DropTable("dbo.StockTakePalletsSnapshots");
        }
    }
}
