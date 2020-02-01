namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timberpropertiesadded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TenantCurrenciesExRates", "Tenant_TenantId", "dbo.Tenants");
            DropIndex("dbo.TenantCurrenciesExRates", new[] { "Tenant_TenantId" });
            AddColumn("dbo.OrderProcessDetails", "FSC", c => c.String());
            AddColumn("dbo.OrderProcessDetails", "PEFC", c => c.String());
            AddColumn("dbo.TenantConfigs", "EnableTimberProperties", c => c.Boolean(nullable: false));
            DropColumn("dbo.TenantConfigs", "ShowDeliveryByGroups");
            DropColumn("dbo.TenantCurrenciesExRates", "Tenant_TenantId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantCurrenciesExRates", "Tenant_TenantId", c => c.Int());
            AddColumn("dbo.TenantConfigs", "ShowDeliveryByGroups", c => c.Boolean(nullable: false));
            DropColumn("dbo.TenantConfigs", "EnableTimberProperties");
            DropColumn("dbo.OrderProcessDetails", "PEFC");
            DropColumn("dbo.OrderProcessDetails", "FSC");
            CreateIndex("dbo.TenantCurrenciesExRates", "Tenant_TenantId");
            AddForeignKey("dbo.TenantCurrenciesExRates", "Tenant_TenantId", "dbo.Tenants", "TenantId");
        }
    }
}
