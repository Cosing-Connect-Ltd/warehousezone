namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantLocationOrderTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "LoyaltyDeliveryOrdersEnabled", c => c.Boolean());
            AddColumn("dbo.TenantLocations", "LoyaltyCollectionOrdersEnabled", c => c.Boolean());
            AddColumn("dbo.TenantLocations", "LoyaltyEatInOrdersEnabled", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "LoyaltyEatInOrdersEnabled");
            DropColumn("dbo.TenantLocations", "LoyaltyCollectionOrdersEnabled");
            DropColumn("dbo.TenantLocations", "LoyaltyDeliveryOrdersEnabled");
        }
    }
}
