namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class propertiesaddintenantwebsitesandcontentpages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "WebsiteContactAddress", c => c.String());
            AddColumn("dbo.TenantWebsites", "WebsiteContactPhone", c => c.String());
            AddColumn("dbo.TenantWebsites", "WebsiteContactEmail", c => c.String());
            AddColumn("dbo.WebsiteContentPages", "pageUrl", c => c.String());
            AddColumn("dbo.WebsiteContentPages", "Content", c => c.String(nullable: false));
            AddColumn("dbo.WebsiteContentPages", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.WebsiteContentPages", "Contant");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebsiteContentPages", "Contant", c => c.String(nullable: false));
            DropColumn("dbo.WebsiteContentPages", "Type");
            DropColumn("dbo.WebsiteContentPages", "Content");
            DropColumn("dbo.WebsiteContentPages", "pageUrl");
            DropColumn("dbo.TenantWebsites", "WebsiteContactEmail");
            DropColumn("dbo.TenantWebsites", "WebsiteContactPhone");
            DropColumn("dbo.TenantWebsites", "WebsiteContactAddress");
        }
    }
}
