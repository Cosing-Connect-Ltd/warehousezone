namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMultiTenancyProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingVouchers", "TenantId", c => c.Int());
            AddColumn("dbo.ShoppingVouchers", "WarehouseId", c => c.Int());
            AddColumn("dbo.ShoppingVouchers", "IsDeleted", c => c.Boolean());
            CreateIndex("dbo.ShoppingVouchers", "TenantId");
            CreateIndex("dbo.ShoppingVouchers", "WarehouseId");
            AddForeignKey("dbo.ShoppingVouchers", "TenantId", "dbo.Tenants", "TenantId");
            AddForeignKey("dbo.ShoppingVouchers", "WarehouseId", "dbo.TenantLocations", "WarehouseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingVouchers", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.ShoppingVouchers", "TenantId", "dbo.Tenants");
            DropIndex("dbo.ShoppingVouchers", new[] { "WarehouseId" });
            DropIndex("dbo.ShoppingVouchers", new[] { "TenantId" });
            DropColumn("dbo.ShoppingVouchers", "IsDeleted");
            DropColumn("dbo.ShoppingVouchers", "WarehouseId");
            DropColumn("dbo.ShoppingVouchers", "TenantId");
        }
    }
}
