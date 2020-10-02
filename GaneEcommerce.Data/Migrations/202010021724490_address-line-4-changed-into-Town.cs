namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addressline4changedintoTown : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShipmentAddressTown", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressTown", c => c.String());
            Sql("UPDATE dbo.Orders SET ShipmentAddressTown = ShipmentAddressLine4");
            Sql("UPDATE dbo.OrderProcesses SET ShipmentAddressTown = ShipmentAddressLine4");
            DropColumn("dbo.AccountAddresses", "AddressLine4");
            DropColumn("dbo.Orders", "ShipmentAddressLine4");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressLine4");
        }

        public override void Down()
        {
            AddColumn("dbo.OrderProcesses", "ShipmentAddressLine4", c => c.String());
            AddColumn("dbo.Orders", "ShipmentAddressLine4", c => c.String());
            AddColumn("dbo.AccountAddresses", "AddressLine4", c => c.String(maxLength: 200));
            Sql("UPDATE dbo.Orders SET ShipmentAddressLine4 = ShipmentAddressTown");
            Sql("UPDATE dbo.OrderProcesses SET ShipmentAddressLine4 = ShipmentAddressTown");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressTown");
            DropColumn("dbo.Orders", "ShipmentAddressTown");
        }
    }
}
