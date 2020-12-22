namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAdyenMetaData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdyenOrderPaylinks",
                c => new
                    {
                        AdyenOrderPaylinkID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        LinkID = c.String(),
                        LinkUrl = c.String(),
                        LinkExpiryDate = c.DateTime(),
                        LinkAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LinkAmountCurrency = c.String(),
                        LinkOrderDescription = c.String(),
                        LinkShopperReference = c.String(),
                        LinkPaymentReference = c.String(),
                        LinkMerchantAccount = c.String(),
                        LinkStorePaymentMethod = c.Boolean(nullable: false),
                        LinkRecurringProcessingModel = c.String(),
                        HookEventCode = c.String(),
                        HookPspReference = c.String(),
                        HookSuccess = c.Boolean(nullable: false),
                        HookAmountCurrency = c.String(),
                        HookAmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HookMerchantOrderReference = c.String(),
                        HookCreatedDate = c.DateTime(nullable: false),
                        RawJson = c.String(),
                    })
                .PrimaryKey(t => t.AdyenOrderPaylinkID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .Index(t => t.OrderID);
            
            AlterColumn("dbo.Feedbacks", "ServiceRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Feedbacks", "FoodRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Feedbacks", "AppRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdyenOrderPaylinks", "OrderID", "dbo.Orders");
            DropIndex("dbo.AdyenOrderPaylinks", new[] { "OrderID" });
            AlterColumn("dbo.Feedbacks", "AppRate", c => c.String());
            AlterColumn("dbo.Feedbacks", "FoodRate", c => c.String());
            AlterColumn("dbo.Feedbacks", "ServiceRate", c => c.String());
            DropTable("dbo.AdyenOrderPaylinks");
        }
    }
}
