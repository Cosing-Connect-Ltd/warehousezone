namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalProcessByPalletMovedtoTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "EnableGlobalProcessByPallet", c => c.Boolean(nullable: false));
            DropColumn("dbo.TenantConfigs", "EnableGlobalProcessByPallet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantConfigs", "EnableGlobalProcessByPallet", c => c.Boolean(nullable: false));
            DropColumn("dbo.TenantLocations", "EnableGlobalProcessByPallet");
        }
    }
}
