namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ShippmentCountry_to_Orders_and_OrdersProcesses_tables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShipmentCountryId", c => c.Int());
            AddColumn("dbo.OrderProcesses", "ShipmentCountryId", c => c.Int());
            CreateIndex("dbo.Orders", "ShipmentCountryId");
            CreateIndex("dbo.OrderProcesses", "ShipmentCountryId");
            AddForeignKey("dbo.OrderProcesses", "ShipmentCountryId", "dbo.GlobalCountry", "CountryID");
            AddForeignKey("dbo.Orders", "ShipmentCountryId", "dbo.GlobalCountry", "CountryID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ShipmentCountryId", "dbo.GlobalCountry");
            DropForeignKey("dbo.OrderProcesses", "ShipmentCountryId", "dbo.GlobalCountry");
            DropIndex("dbo.OrderProcesses", new[] { "ShipmentCountryId" });
            DropIndex("dbo.Orders", new[] { "ShipmentCountryId" });
            DropColumn("dbo.OrderProcesses", "ShipmentCountryId");
            DropColumn("dbo.Orders", "ShipmentCountryId");
        }
    }
}
