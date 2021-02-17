namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RewardsAndVoucherUpdates : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.PalletTrackings", "LocationId", "dbo.Locations");
            //DropForeignKey("dbo.PalletCaseTrackings", "OrderId", "dbo.Orders");
            //DropForeignKey("dbo.PalletCaseTrackings", "PalletTrackingId", "dbo.PalletTrackings");
            //DropForeignKey("dbo.PalletCaseTrackings", "ProductId", "dbo.ProductMaster");
            //DropForeignKey("dbo.PalletCaseTrackings", "TenantId", "dbo.Tenants");
            //DropForeignKey("dbo.PalletCaseTrackings", "WarehouseId", "dbo.TenantLocations");
            //DropIndex("dbo.PalletTrackings", new[] { "LocationId" });
            //DropIndex("dbo.PalletCaseTrackings", new[] { "TenantId" });
            //DropIndex("dbo.PalletCaseTrackings", new[] { "WarehouseId" });
            //DropIndex("dbo.PalletCaseTrackings", new[] { "ProductId" });
            //DropIndex("dbo.PalletCaseTrackings", new[] { "OrderId" });
            //DropIndex("dbo.PalletCaseTrackings", new[] { "PalletTrackingId" });
            AddColumn("dbo.AuthUsers", "PersonalReferralCode", c => c.String());
            AddColumn("dbo.ShoppingVouchers", "RewardDescription", c => c.String());
            AddColumn("dbo.ShoppingVouchers", "MinimumOrderPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShoppingVouchers", "RewardProductId", c => c.Int());
            CreateIndex("dbo.ShoppingVouchers", "RewardProductId");
            AddForeignKey("dbo.ShoppingVouchers", "RewardProductId", "dbo.ProductMaster", "ProductId");
            //DropColumn("dbo.PalletTrackings", "LocationId");
            //DropTable("dbo.PalletCaseTrackings");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.PalletCaseTrackings",
            //    c => new
            //        {
            //            PalletCaseTrackingId = c.Int(nullable: false, identity: true),
            //            PalletCaseSerial = c.String(),
            //            ProductQuantity = c.Int(nullable: false),
            //            Status = c.Int(nullable: false),
            //            TenantId = c.Int(nullable: false),
            //            WarehouseId = c.Int(nullable: false),
            //            ProductId = c.Int(nullable: false),
            //            OrderId = c.Int(),
            //            PalletTrackingId = c.Int(nullable: false),
            //            IsDeleted = c.Boolean(nullable: false),
            //            DateCreated = c.DateTime(nullable: false),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.PalletCaseTrackingId);
            
            //AddColumn("dbo.PalletTrackings", "LocationId", c => c.Int());
            DropForeignKey("dbo.ShoppingVouchers", "RewardProductId", "dbo.ProductMaster");
            DropIndex("dbo.ShoppingVouchers", new[] { "RewardProductId" });
            DropColumn("dbo.ShoppingVouchers", "RewardProductId");
            DropColumn("dbo.ShoppingVouchers", "MinimumOrderPrice");
            DropColumn("dbo.ShoppingVouchers", "RewardDescription");
            DropColumn("dbo.AuthUsers", "PersonalReferralCode");
            //CreateIndex("dbo.PalletCaseTrackings", "PalletTrackingId");
            //CreateIndex("dbo.PalletCaseTrackings", "OrderId");
            //CreateIndex("dbo.PalletCaseTrackings", "ProductId");
            //CreateIndex("dbo.PalletCaseTrackings", "WarehouseId");
            //CreateIndex("dbo.PalletCaseTrackings", "TenantId");
            //CreateIndex("dbo.PalletTrackings", "LocationId");
            //AddForeignKey("dbo.PalletCaseTrackings", "WarehouseId", "dbo.TenantLocations", "WarehouseId");
            //AddForeignKey("dbo.PalletCaseTrackings", "TenantId", "dbo.Tenants", "TenantId");
            //AddForeignKey("dbo.PalletCaseTrackings", "ProductId", "dbo.ProductMaster", "ProductId");
            //AddForeignKey("dbo.PalletCaseTrackings", "PalletTrackingId", "dbo.PalletTrackings", "PalletTrackingId");
            //AddForeignKey("dbo.PalletCaseTrackings", "OrderId", "dbo.Orders", "OrderID");
            //AddForeignKey("dbo.PalletTrackings", "LocationId", "dbo.Locations", "LocationId");
        }
    }
}
