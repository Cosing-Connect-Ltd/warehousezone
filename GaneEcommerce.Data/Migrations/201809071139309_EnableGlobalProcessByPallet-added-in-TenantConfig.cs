namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableGlobalProcessByPalletaddedinTenantConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "EnableGlobalProcessByPallet", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "EnableGlobalProcessByPallet");
        }
    }
}
