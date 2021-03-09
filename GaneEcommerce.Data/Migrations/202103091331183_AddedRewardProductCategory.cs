namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRewardProductCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentPaypalTransactions",
                c => new
                    {
                        PaymentPaypalTransactionId = c.Int(nullable: false, identity: true),
                        PaypalCustomerId = c.String(),
                        TransactionStatus = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        DateSettled = c.DateTime(),
                        SettlementAuthorisationCode = c.String(),
                        PaymentSuccessful = c.Boolean(nullable: false),
                        IsCardFailure = c.Boolean(nullable: false),
                        FailureReasons = c.String(),
                        AcquirerReferenceNumber = c.String(),
                        AuthorizedTransactionId = c.String(),
                    })
                .PrimaryKey(t => t.PaymentPaypalTransactionId);
            
            AddColumn("dbo.ShoppingVouchers", "RewardProductCategory", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingVouchers", "RewardProductCategory");
            DropTable("dbo.PaymentPaypalTransactions");
        }
    }
}
