namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailconfigIdremovedfromNotificationQueue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId", "dbo.TenantEmailConfigs");
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "TenantEmailConfigId" });
            DropColumn("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId", c => c.Int(nullable: false));
            CreateIndex("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId");
            AddForeignKey("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId", "dbo.TenantEmailConfigs", "TenantEmailConfigId");
        }
    }
}
