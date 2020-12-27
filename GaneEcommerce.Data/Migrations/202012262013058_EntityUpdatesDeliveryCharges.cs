namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityUpdatesDeliveryCharges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "StandardDeliveryCost", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.TenantConfigs", "NextDayDeliveryCost", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "NextDayDeliveryCost");
            DropColumn("dbo.TenantConfigs", "StandardDeliveryCost");
        }
    }
}
