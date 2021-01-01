namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountBillingAndDeliveryAddressInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DeliveryAccountAddressID", c => c.Int());
            AddColumn("dbo.Orders", "BillingAccountAddressID", c => c.Int());
            AddColumn("dbo.AccountAddresses", "IsDefaultDeliveryAddress", c => c.Boolean());
            AddColumn("dbo.AccountAddresses", "IsDefaultBillingAddress", c => c.Boolean());
            AddColumn("dbo.OrderProcesses", "DeliveryAccountAddressID", c => c.Int());
            AddColumn("dbo.OrderProcesses", "BillingAccountAddressID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "BillingAccountAddressID");
            DropColumn("dbo.OrderProcesses", "DeliveryAccountAddressID");
            DropColumn("dbo.AccountAddresses", "IsDefaultBillingAddress");
            DropColumn("dbo.AccountAddresses", "IsDefaultDeliveryAddress");
            DropColumn("dbo.Orders", "BillingAccountAddressID");
            DropColumn("dbo.Orders", "DeliveryAccountAddressID");
        }
    }
}
