namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolidateOrderProcessandAllowDuplicateBarcodeflagsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "ConsolidateOrderProcesses", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "AllowDuplicateBarcode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "AllowDuplicateBarcode");
            DropColumn("dbo.TenantLocations", "ConsolidateOrderProcesses");
        }
    }
}
