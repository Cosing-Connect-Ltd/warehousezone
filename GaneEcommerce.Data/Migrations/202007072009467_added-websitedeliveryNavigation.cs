namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedwebsitedeliveryNavigation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteDeliveryNavigations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        TextDescription = c.String(nullable: false),
                        Url = c.String(),
                        IconName = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
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
            DropForeignKey("dbo.WebsiteDeliveryNavigations", "SiteId", "dbo.TenantWebsites");
            DropIndex("dbo.WebsiteDeliveryNavigations", new[] { "SiteId" });
            DropTable("dbo.WebsiteDeliveryNavigations");
        }
    }
}
