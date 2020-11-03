namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpricecalculationconfigstoTenantConfigstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "EnableDynamicPriceCalculation", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "EnableRebateCalculation", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantConfigs", "EnableOrdersHistoryReportDetails", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "EnableOrdersHistoryReportDetails");
            DropColumn("dbo.TenantConfigs", "EnableRebateCalculation");
            DropColumn("dbo.TenantConfigs", "EnableDynamicPriceCalculation");
        }
    }
}
