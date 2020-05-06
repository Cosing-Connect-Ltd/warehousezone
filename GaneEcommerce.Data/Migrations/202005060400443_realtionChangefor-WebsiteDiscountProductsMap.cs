namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realtionChangeforWebsiteDiscountProductsMap : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebsiteDiscountProductsMaps", new[] { "ProductId" });
            RenameColumn(table: "dbo.WebsiteDiscountProductsMaps", name: "ProductId", newName: "ProductMaster_ProductId");
            AddColumn("dbo.WebsiteDiscountProductsMaps", "ProductsWebsitesMapId", c => c.Int(nullable: false));
            AlterColumn("dbo.WebsiteDiscountProductsMaps", "ProductMaster_ProductId", c => c.Int());
            CreateIndex("dbo.WebsiteDiscountProductsMaps", "ProductsWebsitesMapId");
            CreateIndex("dbo.WebsiteDiscountProductsMaps", "ProductMaster_ProductId");
            AddForeignKey("dbo.WebsiteDiscountProductsMaps", "ProductsWebsitesMapId", "dbo.ProductsWebsitesMaps", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteDiscountProductsMaps", "ProductsWebsitesMapId", "dbo.ProductsWebsitesMaps");
            DropIndex("dbo.WebsiteDiscountProductsMaps", new[] { "ProductMaster_ProductId" });
            DropIndex("dbo.WebsiteDiscountProductsMaps", new[] { "ProductsWebsitesMapId" });
            AlterColumn("dbo.WebsiteDiscountProductsMaps", "ProductMaster_ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.WebsiteDiscountProductsMaps", "ProductsWebsitesMapId");
            RenameColumn(table: "dbo.WebsiteDiscountProductsMaps", name: "ProductMaster_ProductId", newName: "ProductId");
            CreateIndex("dbo.WebsiteDiscountProductsMaps", "ProductId");
        }
    }
}
