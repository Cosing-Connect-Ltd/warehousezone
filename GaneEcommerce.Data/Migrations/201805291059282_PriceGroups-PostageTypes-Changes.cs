namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceGroupsPostageTypesChanges : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.OrderDetails set WarrantyID = NULL");
            Sql("delete from dbo.TenantWarranties");
            RenameTable(name: "dbo.PriceGroupDetails", newName: "TenantPriceGroupDetails");
            DropForeignKey("dbo.TenantPostageTypes", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.TenantPostageTypes");
            DropForeignKey("dbo.ProductSerialis", "PostageTypeId", "dbo.TenantPostageTypes");
            DropForeignKey("dbo.ProductPriceLevels", "ProductMasterID", "dbo.ProductMaster");
            DropIndex("dbo.TenantPostageTypes", new[] { "TenantId" });
            DropIndex("dbo.ProductSerialis", new[] { "PostageTypeId" });
            DropIndex("dbo.ProductPriceLevels", new[] { "ProductMasterID" });
            AlterColumn("dbo.AccountContacts", "TenantContactPhone", c => c.String());
            AddForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.OrderConsignmentTypes", "ConsignmentTypeId");
            DropTable("dbo.TenantPostageTypes");
            DropTable("dbo.ProductPriceLevels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductPriceLevels",
                c => new
                    {
                        ProductPriceLevelID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DiscountPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpiryDate = c.DateTime(),
                        ProductMasterID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductPriceLevelID);
            
            CreateTable(
                "dbo.TenantPostageTypes",
                c => new
                    {
                        PostageTypeId = c.Int(nullable: false, identity: true),
                        PostTypeName = c.String(maxLength: 30),
                        Description = c.String(maxLength: 50),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.PostageTypeId);
            
            DropForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.OrderConsignmentTypes");
            AlterColumn("dbo.AccountContacts", "TenantContactPhone", c => c.String(maxLength: 50));
            CreateIndex("dbo.ProductPriceLevels", "ProductMasterID");
            CreateIndex("dbo.ProductSerialis", "PostageTypeId");
            CreateIndex("dbo.TenantPostageTypes", "TenantId");
            AddForeignKey("dbo.ProductPriceLevels", "ProductMasterID", "dbo.ProductMaster", "ProductId");
            AddForeignKey("dbo.ProductSerialis", "PostageTypeId", "dbo.TenantPostageTypes", "PostageTypeId");
            AddForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.TenantPostageTypes", "PostageTypeId");
            AddForeignKey("dbo.TenantPostageTypes", "TenantId", "dbo.Tenants", "TenantId");
            RenameTable(name: "dbo.TenantPriceGroupDetails", newName: "PriceGroupDetails");
        }
    }
}
