namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPaypalColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShopDeliveryTypeID", c => c.Int());
            AddColumn("dbo.Orders", "PaypalBillingAgreementID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaypalBillingAgreementID");
            DropColumn("dbo.Orders", "ShopDeliveryTypeID");
        }
    }
}
