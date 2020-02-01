namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoicemasteridaddedinTenantEmailNotificationQueueTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantEmailNotificationQueues", "InvoiceMasterId", c => c.Int());

        }
        public override void Down()
        {
            DropColumn("dbo.TenantEmailNotificationQueues", "InvoiceMasterId");
        }
    }
}
