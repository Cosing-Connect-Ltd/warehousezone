namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdyenPaymentAndAddressConfigurationChangesRebased : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebsiteWishListItems", new[] { "UserId" });
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
            
            AddColumn("dbo.AuthUsers", "ResetPasswordCode", c => c.String());
            AddColumn("dbo.AuthUsers", "ResetPasswordCodeExpiry", c => c.DateTime());
            AddColumn("dbo.Orders", "BillingAccountAddressID", c => c.Int());
            AddColumn("dbo.AccountAddresses", "IsDefaultDeliveryAddress", c => c.Boolean());
            AddColumn("dbo.AccountAddresses", "IsDefaultBillingAddress", c => c.Boolean());
            AddColumn("dbo.OrderProcesses", "DeliveryAccountAddressID", c => c.Int());
            AddColumn("dbo.OrderProcesses", "BillingAccountAddressID", c => c.Int());
            AddColumn("dbo.WebsiteWishListItems", "EmailId", c => c.String());
            AddColumn("dbo.TenantConfigs", "StandardDeliveryCost", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.TenantConfigs", "NextDayDeliveryCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.WebsiteWishListItems", "UserId", c => c.Int());
            AlterColumn("dbo.Feedbacks", "ServiceRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Feedbacks", "FoodRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Feedbacks", "AppRate", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.Orders", "BillingAccountAddressID");
            CreateIndex("dbo.WebsiteWishListItems", "UserId");
            AddForeignKey("dbo.Orders", "BillingAccountAddressID", "dbo.AccountAddresses", "AddressID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdyenOrderPaylinks", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "BillingAccountAddressID", "dbo.AccountAddresses");
            DropIndex("dbo.AdyenOrderPaylinks", new[] { "OrderID" });
            DropIndex("dbo.WebsiteWishListItems", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "BillingAccountAddressID" });
            AlterColumn("dbo.Feedbacks", "AppRate", c => c.String());
            AlterColumn("dbo.Feedbacks", "FoodRate", c => c.String());
            AlterColumn("dbo.Feedbacks", "ServiceRate", c => c.String());
            AlterColumn("dbo.WebsiteWishListItems", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.TenantConfigs", "NextDayDeliveryCost");
            DropColumn("dbo.TenantConfigs", "StandardDeliveryCost");
            DropColumn("dbo.WebsiteWishListItems", "EmailId");
            DropColumn("dbo.OrderProcesses", "BillingAccountAddressID");
            DropColumn("dbo.OrderProcesses", "DeliveryAccountAddressID");
            DropColumn("dbo.AccountAddresses", "IsDefaultBillingAddress");
            DropColumn("dbo.AccountAddresses", "IsDefaultDeliveryAddress");
            DropColumn("dbo.Orders", "BillingAccountAddressID");
            DropColumn("dbo.AuthUsers", "ResetPasswordCodeExpiry");
            DropColumn("dbo.AuthUsers", "ResetPasswordCode");
            DropTable("dbo.AdyenOrderPaylinks");
            CreateIndex("dbo.WebsiteWishListItems", "UserId");
        }
    }
}
