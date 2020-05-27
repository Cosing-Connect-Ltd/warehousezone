namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUISettingstables : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Tooltips");
            CreateTable(
                "dbo.UISettingItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        DisplayName = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        InputType = c.Int(nullable: false),
                        WebsiteThemeId = c.Int(),
                        WarehouseThemeId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UISettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(),
                        UISettingItemId = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 32),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteId)
                .ForeignKey("dbo.UISettingItems", t => t.UISettingItemId)
                .Index(t => t.SiteId)
                .Index(t => t.UISettingItemId);

            DropColumn("dbo.Tooltips", "TooltipId");
            AddColumn("dbo.Tooltips", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Tooltips", "Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Tooltips", "TooltipId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.UISettings", "UISettingItemId", "dbo.UISettingItems");
            DropForeignKey("dbo.UISettings", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.UISettings", new[] { "UISettingItemId" });
            DropIndex("dbo.UISettings", new[] { "SiteId" });
            DropPrimaryKey("dbo.Tooltips");
            DropColumn("dbo.Tooltips", "Id");
            DropTable("dbo.UISettings");
            DropTable("dbo.UISettingItems");
            AddPrimaryKey("dbo.Tooltips", "TooltipId");
        }
    }
}
