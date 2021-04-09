namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTenantPriceGroupToSpecialPriceKeysUpdated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSpecialAttributePrices",
                c => new
                    {
                        ProductSpecialAttributePriceId = c.Int(nullable: false, identity: true),
                        ProductAttributeId = c.Int(nullable: false),
                        ProductAttributeType = c.Int(nullable: false),
                        ProductAttributeValueId = c.Int(nullable: false),
                        AttributeSpecificPrice = c.Decimal(precision: 18, scale: 2),
                        SortOrder = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PriceGroupID = c.Int(nullable: false),
                        PriceGroupDetailID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductSpecialAttributePriceId)
                .ForeignKey("dbo.TenantPriceGroups", t => t.PriceGroupID)
                .ForeignKey("dbo.TenantPriceGroupDetails", t => t.PriceGroupDetailID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductAttributes", t => t.ProductAttributeId)
                .ForeignKey("dbo.ProductAttributeValues", t => t.ProductAttributeValueId)
                .Index(t => t.ProductAttributeId)
                .Index(t => t.ProductAttributeValueId)
                .Index(t => t.ProductId)
                .Index(t => t.PriceGroupID)
                .Index(t => t.PriceGroupDetailID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSpecialAttributePrices", "ProductAttributeValueId", "dbo.ProductAttributeValues");
            DropForeignKey("dbo.ProductSpecialAttributePrices", "ProductAttributeId", "dbo.ProductAttributes");
            DropForeignKey("dbo.ProductSpecialAttributePrices", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductSpecialAttributePrices", "PriceGroupDetailID", "dbo.TenantPriceGroupDetails");
            DropForeignKey("dbo.ProductSpecialAttributePrices", "PriceGroupID", "dbo.TenantPriceGroups");
            DropIndex("dbo.ProductSpecialAttributePrices", new[] { "PriceGroupDetailID" });
            DropIndex("dbo.ProductSpecialAttributePrices", new[] { "PriceGroupID" });
            DropIndex("dbo.ProductSpecialAttributePrices", new[] { "ProductId" });
            DropIndex("dbo.ProductSpecialAttributePrices", new[] { "ProductAttributeValueId" });
            DropIndex("dbo.ProductSpecialAttributePrices", new[] { "ProductAttributeId" });
            DropTable("dbo.ProductSpecialAttributePrices");
        }
    }
}
