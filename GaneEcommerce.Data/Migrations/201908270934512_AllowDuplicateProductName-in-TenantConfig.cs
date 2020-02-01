namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowDuplicateProductNameinTenantConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "AllowDuplicateProductName", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "AllowDuplicateProductName");
        }
    }
}
