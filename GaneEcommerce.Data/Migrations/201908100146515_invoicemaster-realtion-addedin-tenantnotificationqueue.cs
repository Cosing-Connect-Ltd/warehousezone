namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoicemasterrealtionaddedintenantnotificationqueue : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TenantEmailNotificationQueues", "InvoiceMasterId");
            AddForeignKey("dbo.TenantEmailNotificationQueues", "InvoiceMasterId", "dbo.InvoiceMaster", "InvoiceMasterId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenantEmailNotificationQueues", "InvoiceMasterId", "dbo.InvoiceMaster");
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "InvoiceMasterId" });
        }
    }
}
