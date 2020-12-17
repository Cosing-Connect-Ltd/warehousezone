namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shipmentaddressnameaddedinorderstables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShipmentAddressName", c => c.String());
            AddColumn("dbo.OrderProcesses", "ShipmentAddressName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "ShipmentAddressName");
            DropColumn("dbo.Orders", "ShipmentAddressName");
        }
    }
}
