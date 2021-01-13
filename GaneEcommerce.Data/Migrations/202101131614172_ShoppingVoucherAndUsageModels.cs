namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShoppingVoucherAndUsageModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShoppingVouchers",
                c => new
                    {
                        ShoppingVoucherId = c.Int(nullable: false, identity: true),
                        VoucherTitle = c.String(),
                        VoucherCode = c.String(),
                        VoucherUserId = c.Int(),
                        DiscountType = c.Int(nullable: false),
                        DiscountFigure = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VoucherUsedDate = c.DateTime(),
                        VoucherExpiryDate = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ShoppingVoucherId)
                .ForeignKey("dbo.AuthUsers", t => t.VoucherUserId)
                .Index(t => t.VoucherUserId);
            
            CreateTable(
                "dbo.ShoppingVoucherUsages",
                c => new
                    {
                        ShoppingVoucherUsageId = c.Int(nullable: false, identity: true),
                        ShoppingVoucherId = c.Int(nullable: false),
                        VoucherUserId = c.Int(),
                        OrderId = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ShoppingVoucherUsageId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.ShoppingVouchers", t => t.ShoppingVoucherId)
                .ForeignKey("dbo.AuthUsers", t => t.VoucherUserId)
                .Index(t => t.ShoppingVoucherId)
                .Index(t => t.VoucherUserId)
                .Index(t => t.OrderId);
            
            AddColumn("dbo.Orders", "ShoppingVoucherId", c => c.Int());
            AddColumn("dbo.Orders", "VoucherCode", c => c.String());
            AddColumn("dbo.Orders", "VoucherCodeDiscount", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.Orders", "ShoppingVoucherId");
            AddForeignKey("dbo.Orders", "ShoppingVoucherId", "dbo.ShoppingVouchers", "ShoppingVoucherId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ShoppingVoucherId", "dbo.ShoppingVouchers");
            DropForeignKey("dbo.ShoppingVouchers", "VoucherUserId", "dbo.AuthUsers");
            DropForeignKey("dbo.ShoppingVoucherUsages", "VoucherUserId", "dbo.AuthUsers");
            DropForeignKey("dbo.ShoppingVoucherUsages", "ShoppingVoucherId", "dbo.ShoppingVouchers");
            DropForeignKey("dbo.ShoppingVoucherUsages", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingVoucherUsages", new[] { "OrderId" });
            DropIndex("dbo.ShoppingVoucherUsages", new[] { "VoucherUserId" });
            DropIndex("dbo.ShoppingVoucherUsages", new[] { "ShoppingVoucherId" });
            DropIndex("dbo.ShoppingVouchers", new[] { "VoucherUserId" });
            DropIndex("dbo.Orders", new[] { "ShoppingVoucherId" });
            DropColumn("dbo.Orders", "VoucherCodeDiscount");
            DropColumn("dbo.Orders", "VoucherCode");
            DropColumn("dbo.Orders", "ShoppingVoucherId");
            DropTable("dbo.ShoppingVoucherUsages");
            DropTable("dbo.ShoppingVouchers");
        }
    }
}
