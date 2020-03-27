namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeProductReceipeMastersandaddedProductKitEnumType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductReceipeMasters", "RecipeItemProductID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductReceipeMasters", "ProductMasterID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductReceipeMasters", "ProductMaster_ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductKitMaps", "KitProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductKitMaps", new[] { "KitProductId" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "ProductMasterID" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "RecipeItemProductID" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "ProductMaster_ProductId" });
            RenameColumn(table: "dbo.ProductKitMaps", name: "ProductMaster_ProductId", newName: "KitProductMaster_ProductId");
            RenameIndex(table: "dbo.ProductKitMaps", name: "IX_ProductMaster_ProductId", newName: "IX_KitProductMaster_ProductId");
            AddColumn("dbo.ProductKitMaps", "ProductKitType", c => c.Int(nullable: false));
            AddForeignKey("dbo.ProductKitMaps", "KitProductMaster_ProductId", "dbo.ProductMaster", "ProductId");
            DropTable("dbo.ProductReceipeMasters");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductReceipeMasters",
                c => new
                    {
                        ProductReceipeMasterID = c.Int(nullable: false, identity: true),
                        ProductMasterID = c.Int(nullable: false),
                        RecipeItemProductID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        ProductMaster_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductReceipeMasterID);
            
            DropForeignKey("dbo.ProductKitMaps", "KitProductMaster_ProductId", "dbo.ProductMaster");
            DropColumn("dbo.ProductKitMaps", "ProductKitType");
            RenameIndex(table: "dbo.ProductKitMaps", name: "IX_KitProductMaster_ProductId", newName: "IX_ProductMaster_ProductId");
            RenameColumn(table: "dbo.ProductKitMaps", name: "KitProductMaster_ProductId", newName: "ProductMaster_ProductId");
            CreateIndex("dbo.ProductReceipeMasters", "ProductMaster_ProductId");
            CreateIndex("dbo.ProductReceipeMasters", "RecipeItemProductID");
            CreateIndex("dbo.ProductReceipeMasters", "ProductMasterID");
            CreateIndex("dbo.ProductKitMaps", "KitProductId");
            AddForeignKey("dbo.ProductKitMaps", "KitProductId", "dbo.ProductMaster", "ProductId");
            AddForeignKey("dbo.ProductReceipeMasters", "ProductMaster_ProductId", "dbo.ProductMaster", "ProductId");
            AddForeignKey("dbo.ProductReceipeMasters", "ProductMasterID", "dbo.ProductMaster", "ProductId");
            AddForeignKey("dbo.ProductReceipeMasters", "RecipeItemProductID", "dbo.ProductMaster", "ProductId");
        }
    }
}
