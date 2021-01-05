namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncludedShopDeliveryTypeToOrder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "DeliveryAccountAddressID", "dbo.AccountAddresses");
            DropIndex("dbo.Orders", new[] { "DeliveryAccountAddressID" });
            DropColumn("dbo.Orders", "DeliveryAccountAddressID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "DeliveryAccountAddressID", c => c.Int());
            CreateIndex("dbo.Orders", "DeliveryAccountAddressID");
            AddForeignKey("dbo.Orders", "DeliveryAccountAddressID", "dbo.AccountAddresses", "AddressID");
        }
    }
}
