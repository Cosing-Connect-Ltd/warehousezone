namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deliverycollectionchargespropertiesaddedinTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "DeliveryCharges", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TenantLocations", "CollectionCharges", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TenantLocations", "DeliveryRadiusMiles", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "DeliveryRadiusMiles");
            DropColumn("dbo.TenantLocations", "CollectionCharges");
            DropColumn("dbo.TenantLocations", "DeliveryCharges");
        }
    }
}
