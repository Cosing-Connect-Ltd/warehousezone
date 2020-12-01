namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsInternalWebsitefieldtoTenantWebsitestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "IsInternalWebsite", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "IsInternalWebsite");
        }
    }
}
