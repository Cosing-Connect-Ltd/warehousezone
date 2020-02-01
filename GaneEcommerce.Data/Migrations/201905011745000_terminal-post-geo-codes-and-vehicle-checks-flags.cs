namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class terminalpostgeocodesandvehiclechecksflags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Terminals", "VehicleChecksAtStart", c => c.Boolean(nullable: false));
            AddColumn("dbo.Terminals", "PostGeoLocation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Terminals", "PostGeoLocation");
            DropColumn("dbo.Terminals", "VehicleChecksAtStart");
        }
    }
}
