namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dpdResonsecoulmnaddedindatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletsDispatches", "NetworkCode", c => c.String());
            AddColumn("dbo.PalletsDispatches", "ShipmentId", c => c.String());
            AddColumn("dbo.PalletsDispatches", "ConsignmentNumber", c => c.String());
            AddColumn("dbo.PalletsDispatches", "ParcelNumbers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletsDispatches", "ParcelNumbers");
            DropColumn("dbo.PalletsDispatches", "ConsignmentNumber");
            DropColumn("dbo.PalletsDispatches", "ShipmentId");
            DropColumn("dbo.PalletsDispatches", "NetworkCode");
        }
    }
}
