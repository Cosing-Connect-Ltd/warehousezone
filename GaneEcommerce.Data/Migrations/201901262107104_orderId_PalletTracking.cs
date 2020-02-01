namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderId_PalletTracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletTrackings", "OrderId", c => c.Int());
            CreateIndex("dbo.PalletTrackings", "OrderId");
            AddForeignKey("dbo.PalletTrackings", "OrderId", "dbo.Orders", "OrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PalletTrackings", "OrderId", "dbo.Orders");
            DropIndex("dbo.PalletTrackings", new[] { "OrderId" });
            DropColumn("dbo.PalletTrackings", "OrderId");
        }
    }
}
