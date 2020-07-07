namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWebsiteLayoutSettingstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteLayoutSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        SubscriptionPanelTitle = c.String(),
                        SubscriptionPanelDescription = c.String(),
                        SubscriptionPanelImageUrl = c.String(),
                        SubscriptionHandlerUrl = c.String(),
                        ShowSubscriptionPanel = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteId)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteLayoutSettings", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.WebsiteLayoutSettings", new[] { "SiteId" });
            DropTable("dbo.WebsiteLayoutSettings");
        }
    }
}
