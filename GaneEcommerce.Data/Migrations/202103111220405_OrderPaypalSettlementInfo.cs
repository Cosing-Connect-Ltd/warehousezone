namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderPaypalSettlementInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PaypalBraintreeNonce", c => c.String());
            AddColumn("dbo.Orders", "PaypalTransactionFee", c => c.String());
            AddColumn("dbo.Orders", "PaypalPaymentId", c => c.String());
            AddColumn("dbo.Orders", "PaypalPayerEmail", c => c.String());
            AddColumn("dbo.Orders", "PaypalPayerFirstName", c => c.String());
            AddColumn("dbo.Orders", "PaypalPayerSurname", c => c.String());
            AddColumn("dbo.Orders", "PaypalAuthorizationId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaypalAuthorizationId");
            DropColumn("dbo.Orders", "PaypalPayerSurname");
            DropColumn("dbo.Orders", "PaypalPayerFirstName");
            DropColumn("dbo.Orders", "PaypalPayerEmail");
            DropColumn("dbo.Orders", "PaypalPaymentId");
            DropColumn("dbo.Orders", "PaypalTransactionFee");
            DropColumn("dbo.Orders", "PaypalBraintreeNonce");
        }
    }
}
