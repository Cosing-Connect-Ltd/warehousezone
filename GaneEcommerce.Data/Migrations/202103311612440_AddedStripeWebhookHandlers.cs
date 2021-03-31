namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStripeWebhookHandlers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StripeChargeInformations", "StripeChargePendingDate", c => c.DateTime());
            AddColumn("dbo.StripeChargeInformations", "StripeChargeCapturedDate", c => c.DateTime());
            AddColumn("dbo.StripeChargeInformations", "StripeChargeSucceededDate", c => c.DateTime());
            AddColumn("dbo.StripeChargeInformations", "StripeChargeRefundedDate", c => c.DateTime());
            AddColumn("dbo.StripeChargeInformations", "StripeChargeFailedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StripeChargeInformations", "StripeChargeFailedDate");
            DropColumn("dbo.StripeChargeInformations", "StripeChargeRefundedDate");
            DropColumn("dbo.StripeChargeInformations", "StripeChargeSucceededDate");
            DropColumn("dbo.StripeChargeInformations", "StripeChargeCapturedDate");
            DropColumn("dbo.StripeChargeInformations", "StripeChargePendingDate");
        }
    }
}
