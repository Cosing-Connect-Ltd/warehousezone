namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class siteIdaddedinEmailTempaltes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantEmailTemplates", "SiteId", c => c.Int());
            CreateIndex("dbo.TenantEmailTemplates", "SiteId");
            AddForeignKey("dbo.TenantEmailTemplates", "SiteId", "dbo.TenantWebsites", "SiteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenantEmailTemplates", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.TenantEmailTemplates", new[] { "SiteId" });
            DropColumn("dbo.TenantEmailTemplates", "SiteId");
        }
    }
}
