namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedForeignKeysForAccountAddresses : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Orders", "DeliveryAccountAddressID");
            CreateIndex("dbo.Orders", "BillingAccountAddressID");
            AddForeignKey("dbo.Orders", "BillingAccountAddressID", "dbo.AccountAddresses", "AddressID");
            AddForeignKey("dbo.Orders", "DeliveryAccountAddressID", "dbo.AccountAddresses", "AddressID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "DeliveryAccountAddressID", "dbo.AccountAddresses");
            DropForeignKey("dbo.Orders", "BillingAccountAddressID", "dbo.AccountAddresses");
            DropIndex("dbo.Orders", new[] { "BillingAccountAddressID" });
            DropIndex("dbo.Orders", new[] { "DeliveryAccountAddressID" });
        }
    }
}
