namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductMasterProductCategoryRelation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductMaster", "ProductCategoryId");
            AddForeignKey("dbo.ProductMaster", "ProductCategoryId", "dbo.ProductCategories", "ProductCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductMaster", "ProductCategoryId", "dbo.ProductCategories");
            DropIndex("dbo.ProductMaster", new[] { "ProductCategoryId" });
        }
    }
}
