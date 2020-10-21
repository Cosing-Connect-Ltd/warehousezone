namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedorderupdatenotificationrelatedfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "SendOrderStatusByEmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "SendOrderStatusBySms", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "NotifiableOrderStatuses", c => c.String());
            AddColumn("dbo.Orders", "SendOrderStatusByEmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "SendOrderStatusBySms", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "AllowOrderStatusEmailConfigChange", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "AllowOrderStatusSmsConfigChange", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "AllowOrderStatusSmsConfigChange");
            DropColumn("dbo.TenantConfigs", "AllowOrderStatusEmailConfigChange");
            DropColumn("dbo.Orders", "SendOrderStatusBySms");
            DropColumn("dbo.Orders", "SendOrderStatusByEmail");
            DropColumn("dbo.TenantLocations", "NotifiableOrderStatuses");
            DropColumn("dbo.TenantLocations", "SendOrderStatusBySms");
            DropColumn("dbo.TenantLocations", "SendOrderStatusByEmail");
        }
    }
}
