namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPriceGrouptoTenantWebsitestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "DefaultPriceGroupId", c => c.Int());
            CreateIndex("dbo.TenantWebsites", "DefaultPriceGroupId");
            AddForeignKey("dbo.TenantWebsites", "DefaultPriceGroupId", "dbo.TenantPriceGroups", "PriceGroupID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenantWebsites", "DefaultPriceGroupId", "dbo.TenantPriceGroups");
            DropIndex("dbo.TenantWebsites", new[] { "DefaultPriceGroupId" });
            DropColumn("dbo.TenantWebsites", "DefaultPriceGroupId");
        }
    }
}
