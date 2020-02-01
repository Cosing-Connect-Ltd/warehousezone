namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDetailIdchangedtoOrderProcessDetailIdinPalletProducts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PalletProducts", "OrderDetailID", "dbo.OrderDetails");
            DropIndex("dbo.PalletProducts", new[] { "OrderDetailID" });
            AddColumn("dbo.PalletProducts", "OrderProcessDetailID", c => c.Int());
            CreateIndex("dbo.PalletProducts", "OrderProcessDetailID");
            AddForeignKey("dbo.PalletProducts", "OrderProcessDetailID", "dbo.OrderProcessDetails", "OrderProcessDetailID");
            DropColumn("dbo.PalletProducts", "OrderDetailID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PalletProducts", "OrderDetailID", c => c.Int(nullable: false));
            DropForeignKey("dbo.PalletProducts", "OrderProcessDetailID", "dbo.OrderProcessDetails");
            DropIndex("dbo.PalletProducts", new[] { "OrderProcessDetailID" });
            DropColumn("dbo.PalletProducts", "OrderProcessDetailID");
            CreateIndex("dbo.PalletProducts", "OrderDetailID");
            AddForeignKey("dbo.PalletProducts", "OrderDetailID", "dbo.OrderDetails", "OrderDetailID");
        }
    }
}
