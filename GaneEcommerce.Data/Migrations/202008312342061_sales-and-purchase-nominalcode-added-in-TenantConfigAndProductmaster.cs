namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesandpurchasenominalcodeaddedinTenantConfigAndProductmaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "SaleNominalCode", c => c.Int());
            AddColumn("dbo.ProductMaster", "PurchaseNominalCode", c => c.Int());
            AddColumn("dbo.TenantConfigs", "SaleNominalCode", c => c.Int());
            AddColumn("dbo.TenantConfigs", "PurchaseNominalCode", c => c.Int());
            DropColumn("dbo.ProductMaster", "NominalCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductMaster", "NominalCode", c => c.Int());
            DropColumn("dbo.TenantConfigs", "PurchaseNominalCode");
            DropColumn("dbo.TenantConfigs", "SaleNominalCode");
            DropColumn("dbo.ProductMaster", "PurchaseNominalCode");
            DropColumn("dbo.ProductMaster", "SaleNominalCode");
        }
    }
}
