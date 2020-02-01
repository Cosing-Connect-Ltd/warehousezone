namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantUserTimeZoneId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "UserCulture", c => c.String());
            AddColumn("dbo.AuthUsers", "UserTimeZoneId", c => c.String());
            AddColumn("dbo.Tenants", "TenantTimeZoneId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tenants", "TenantTimeZoneId");
            DropColumn("dbo.AuthUsers", "UserTimeZoneId");
            DropColumn("dbo.AuthUsers", "UserCulture");
        }
    }
}
