namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class homepagedetailsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "HomeTopCategoryText", c => c.String());
            AddColumn("dbo.TenantWebsites", "HomeFeaturedProductText", c => c.String());
            AddColumn("dbo.TenantWebsites", "FeaturedTagId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "FeaturedTagId");
            DropColumn("dbo.TenantWebsites", "HomeFeaturedProductText");
            DropColumn("dbo.TenantWebsites", "HomeTopCategoryText");
        }
    }
}
