namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderIdnullableinTenantEmailNotificationQueue : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "OrderId" });
            AlterColumn("dbo.TenantEmailNotificationQueues", "OrderId", c => c.Int());
            CreateIndex("dbo.TenantEmailNotificationQueues", "OrderId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "OrderId" });
            AlterColumn("dbo.TenantEmailNotificationQueues", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.TenantEmailNotificationQueues", "OrderId");
        }
    }
}
