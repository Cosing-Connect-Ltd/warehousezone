namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addednewconfigstoTenantConfigstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "ShowDeliveryServiceInOrdersList", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "ShowExternalShopSiteNameInOrdersList", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "ShowExternalShopSiteNameInOrdersList");
            DropColumn("dbo.TenantConfigs", "ShowDeliveryServiceInOrdersList");
        }
    }
}
