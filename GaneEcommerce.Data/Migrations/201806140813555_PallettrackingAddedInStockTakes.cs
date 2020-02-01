namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PallettrackingAddedInStockTakes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockTakeDetailsPallets",
                c => new
                    {
                        StockTakeDetailPalletId = c.Int(nullable: false, identity: true),
                        ProductPalletId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PalletSerial = c.String(),
                        StockTakeDetailId = c.Int(nullable: false),
                        DateScanned = c.DateTime(nullable: false),
                        CreatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.StockTakeDetailPalletId)
                .ForeignKey("dbo.PalletTrackings", t => t.ProductPalletId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.StockTakeDetails", t => t.StockTakeDetailId)
                .Index(t => t.ProductPalletId)
                .Index(t => t.ProductId)
                .Index(t => t.StockTakeDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockTakeDetailsPallets", "StockTakeDetailId", "dbo.StockTakeDetails");
            DropForeignKey("dbo.StockTakeDetailsPallets", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.StockTakeDetailsPallets", "ProductPalletId", "dbo.PalletTrackings");
            DropIndex("dbo.StockTakeDetailsPallets", new[] { "StockTakeDetailId" });
            DropIndex("dbo.StockTakeDetailsPallets", new[] { "ProductId" });
            DropIndex("dbo.StockTakeDetailsPallets", new[] { "ProductPalletId" });
            DropTable("dbo.StockTakeDetailsPallets");
        }
    }
}
