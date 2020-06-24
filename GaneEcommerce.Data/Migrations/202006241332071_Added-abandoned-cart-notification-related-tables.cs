namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedabandonedcartnotificationrelatedtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AbandonedCartNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                        SendDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AbandonedCartSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        NotificationEmailTemplate = c.String(),
                        NotificationEmailSubjectTemplate = c.String(),
                        NotificationDelay = c.Int(nullable: false),
                        IsNotificationEnabled = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AbandonedCartSettings", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.AbandonedCartSettings", new[] { "SiteID" });
            DropTable("dbo.AbandonedCartSettings");
            DropTable("dbo.AbandonedCartNotifications");
        }
    }
}
