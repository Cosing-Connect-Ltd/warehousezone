namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenantRelationaddedinproductManufacturer : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductManufacturers", "TenantId");
            AddForeignKey("dbo.ProductManufacturers", "TenantId", "dbo.Tenants", "TenantId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductManufacturers", "TenantId", "dbo.Tenants");
            DropIndex("dbo.ProductManufacturers", new[] { "TenantId" });
        }
    }
}
