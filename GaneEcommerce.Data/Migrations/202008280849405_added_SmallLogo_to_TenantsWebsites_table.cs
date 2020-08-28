namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_SmallLogo_to_TenantsWebsites_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "SmallLogo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "SmallLogo");
        }
    }
}
