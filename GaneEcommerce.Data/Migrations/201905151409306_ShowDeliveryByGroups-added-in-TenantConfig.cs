namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowDeliveryByGroupsaddedinTenantConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "ShowDeliveryByGroups", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "ShowDeliveryByGroups");
        }
    }
}
