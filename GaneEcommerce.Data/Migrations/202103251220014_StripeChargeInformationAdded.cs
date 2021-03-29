namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StripeChargeInformationAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StripeChargeInformations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StripeChargedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StripeChargedCurrency = c.String(),
                        StripeChargeCreatedId = c.String(),
                        StripeChargeToken = c.String(),
                        StripeAutoCharge = c.Boolean(nullable: false),
                        StripeChargedCreatedDate = c.DateTime(),
                        StripeChargedConfirmedDate = c.DateTime(),
                        OrderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "StripeChargeConfirmationId", c => c.Int());
            CreateIndex("dbo.Orders", "StripeChargeConfirmationId");
            AddForeignKey("dbo.Orders", "StripeChargeConfirmationId", "dbo.StripeChargeInformations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "StripeChargeConfirmationId", "dbo.StripeChargeInformations");
            DropIndex("dbo.Orders", new[] { "StripeChargeConfirmationId" });
            DropColumn("dbo.Orders", "StripeChargeConfirmationId");
            DropTable("dbo.StripeChargeInformations");
        }
    }
}
