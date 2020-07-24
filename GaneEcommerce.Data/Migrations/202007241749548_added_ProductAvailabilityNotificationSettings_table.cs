namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class added_ProductAvailabilityNotificationSettings_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAvailabilityNotificationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        NotificationEmailTemplate = c.String(),
                        NotificationEmailSubjectTemplate = c.String(),
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

            CreateIndex("dbo.ProductAvailabilityNotifyQueues", "ProductId");
            AddForeignKey("dbo.ProductAvailabilityNotifyQueues", "ProductId", "dbo.ProductMaster", "ProductId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProductAvailabilityNotifyQueues", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAvailabilityNotificationSettings", "SiteID", "dbo.TenantWebsites");
            DropIndex("dbo.ProductAvailabilityNotifyQueues", new[] { "ProductId" });
            DropIndex("dbo.ProductAvailabilityNotificationSettings", new[] { "SiteID" });
            DropTable("dbo.ProductAvailabilityNotificationSettings");
        }
    }
}
