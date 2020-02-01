namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blindshipmentflagsintenantlocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "ShowTaxInBlindShipment", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "ShowPriceInBlindShipment", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "ShowQtyInBlindShipment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "ShowQtyInBlindShipment");
            DropColumn("dbo.TenantLocations", "ShowPriceInBlindShipment");
            DropColumn("dbo.TenantLocations", "ShowTaxInBlindShipment");
        }
    }
}
