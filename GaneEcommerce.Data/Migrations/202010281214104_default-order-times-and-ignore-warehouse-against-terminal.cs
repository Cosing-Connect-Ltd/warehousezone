namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class defaultordertimesandignorewarehouseagainstterminal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "DefaultDeliveryTimeMinutes", c => c.Int());
            AddColumn("dbo.TenantLocations", "DefaultCollectionTimeMinutes", c => c.Int());
            AddColumn("dbo.TenantLocations", "DefaultEatInTimeMinutes", c => c.Int());
            AddColumn("dbo.Terminals", "IgnoreWarehouseForOrderPost", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Terminals", "IgnoreWarehouseForOrderPost");
            DropColumn("dbo.TenantLocations", "DefaultEatInTimeMinutes");
            DropColumn("dbo.TenantLocations", "DefaultCollectionTimeMinutes");
            DropColumn("dbo.TenantLocations", "DefaultDeliveryTimeMinutes");
        }
    }
}
