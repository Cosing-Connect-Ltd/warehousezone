namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SiteIdinAuthUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "SiteId", c => c.Int());
            CreateIndex("dbo.AuthUsers", "SiteId");
            AddForeignKey("dbo.AuthUsers", "SiteId", "dbo.TenantWebsites", "SiteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthUsers", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.AuthUsers", new[] { "SiteId" });
            DropColumn("dbo.AuthUsers", "SiteId");
        }
    }
}
