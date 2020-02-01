namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleDriverIdnullableinVehicleInspection : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.VehicleInspection", new[] { "VehicleDriverId" });
            AlterColumn("dbo.VehicleInspection", "VehicleDriverId", c => c.Int());
            CreateIndex("dbo.VehicleInspection", "VehicleDriverId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VehicleInspection", new[] { "VehicleDriverId" });
            AlterColumn("dbo.VehicleInspection", "VehicleDriverId", c => c.Int(nullable: false));
            CreateIndex("dbo.VehicleInspection", "VehicleDriverId");
        }
    }
}
