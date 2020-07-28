namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFaviconcolumntoTenantWebsitestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "Favicon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "Favicon");
        }
    }
}
