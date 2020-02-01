namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShipmentDetailsToOrderProcess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcesses", "ShipmentAddressLine1", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressLine2", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressLine3", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressLine4", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressPostcode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "ShipmentAddressPostcode");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressLine4");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressLine3");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressLine2");
            DropColumn("dbo.OrderProcesses", "ShipmentAddressLine1");
        }
    }
}
