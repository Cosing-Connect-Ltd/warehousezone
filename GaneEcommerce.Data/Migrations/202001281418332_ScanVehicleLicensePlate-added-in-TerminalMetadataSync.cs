namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScanVehicleLicensePlateaddedinTerminalMetadataSync : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Terminals", "ScanVehicleLicensePlate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Terminals", "ScanVehicleLicensePlate");
        }
    }
}
