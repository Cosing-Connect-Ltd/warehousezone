namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morepropertiesaddedincontentpages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteNavigations", "ShowInNavigation", c => c.Boolean(nullable: false));
            AddColumn("dbo.WebsiteNavigations", "ShowInFooter", c => c.Boolean(nullable: false));
            AlterColumn("dbo.WebsiteContentPages", "pageUrl", c => c.String(nullable: false));
            CreateIndex("dbo.WebsiteNavigations", "ContentPageId");
            AddForeignKey("dbo.WebsiteNavigations", "ContentPageId", "dbo.WebsiteContentPages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteNavigations", "ContentPageId", "dbo.WebsiteContentPages");
            DropIndex("dbo.WebsiteNavigations", new[] { "ContentPageId" });
            AlterColumn("dbo.WebsiteContentPages", "pageUrl", c => c.String());
            DropColumn("dbo.WebsiteNavigations", "ShowInFooter");
            DropColumn("dbo.WebsiteNavigations", "ShowInNavigation");
        }
    }
}
