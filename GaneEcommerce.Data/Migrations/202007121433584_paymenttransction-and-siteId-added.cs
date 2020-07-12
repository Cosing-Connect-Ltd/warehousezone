namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymenttransctionandsiteIdadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantEmailConfigs", "SiteId", c => c.Int());
            AddColumn("dbo.AccountTransaction", "PaymentTransactionId", c => c.String());
            AddColumn("dbo.TenantConfigs", "SiteId", c => c.Int());
            CreateIndex("dbo.TenantEmailConfigs", "SiteId");
            CreateIndex("dbo.TenantConfigs", "SiteId");
            AddForeignKey("dbo.TenantEmailConfigs", "SiteId", "dbo.TenantWebsites", "SiteID");
            AddForeignKey("dbo.TenantConfigs", "SiteId", "dbo.TenantWebsites", "SiteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenantConfigs", "SiteId", "dbo.TenantWebsites");
            DropForeignKey("dbo.TenantEmailConfigs", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.TenantConfigs", new[] { "SiteId" });
            DropIndex("dbo.TenantEmailConfigs", new[] { "SiteId" });
            DropColumn("dbo.TenantConfigs", "SiteId");
            DropColumn("dbo.AccountTransaction", "PaymentTransactionId");
            DropColumn("dbo.TenantEmailConfigs", "SiteId");
        }
    }
}
