namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRefundRequestDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdyenOrderPaylinks", "HookRawJson", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundRequestedUserID", c => c.Int());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundMerchantReference", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundOriginalMerchantReference", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundRequestedAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AdyenOrderPaylinks", "RefundRequestedAmountCurrency", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundRequestedDateTime", c => c.DateTime());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundProcessedDateTime", c => c.DateTime());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookEventCode", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookPspReference", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookSuccess", c => c.Boolean());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookAmountCurrency", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookAmountPaid", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookMerchantOrderReference", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookCreatedDate", c => c.DateTime());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundHookReason", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundResponseStatus", c => c.String());
            AddColumn("dbo.AdyenOrderPaylinks", "RefundResponsePspReference", c => c.String());
            DropColumn("dbo.AdyenOrderPaylinks", "RawJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdyenOrderPaylinks", "RawJson", c => c.String());
            DropColumn("dbo.AdyenOrderPaylinks", "RefundResponsePspReference");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundResponseStatus");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookReason");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookCreatedDate");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookMerchantOrderReference");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookAmountPaid");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookAmountCurrency");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookSuccess");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookPspReference");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundHookEventCode");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundProcessedDateTime");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundRequestedDateTime");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundRequestedAmountCurrency");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundRequestedAmount");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundOriginalMerchantReference");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundMerchantReference");
            DropColumn("dbo.AdyenOrderPaylinks", "RefundRequestedUserID");
            DropColumn("dbo.AdyenOrderPaylinks", "HookRawJson");
        }
    }
}
