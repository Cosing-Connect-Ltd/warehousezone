namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStripeRefundInformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StripeChargeInformations", "RefundId", c => c.String());
            AddColumn("dbo.StripeChargeInformations", "RefundBalanceTransactionId", c => c.String());
            AddColumn("dbo.StripeChargeInformations", "RefundCreatedDate", c => c.DateTime());
            AddColumn("dbo.StripeChargeInformations", "RefundAmount", c => c.Long());
            AddColumn("dbo.StripeChargeInformations", "RefundAmountCurrency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StripeChargeInformations", "RefundAmountCurrency");
            DropColumn("dbo.StripeChargeInformations", "RefundAmount");
            DropColumn("dbo.StripeChargeInformations", "RefundCreatedDate");
            DropColumn("dbo.StripeChargeInformations", "RefundBalanceTransactionId");
            DropColumn("dbo.StripeChargeInformations", "RefundId");
        }
    }
}
