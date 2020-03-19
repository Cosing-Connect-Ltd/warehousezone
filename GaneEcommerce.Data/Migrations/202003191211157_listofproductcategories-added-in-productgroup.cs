namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class listofproductcategoriesaddedinproductgroup : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductCategories", new[] { "ProductGroupId" });
            AlterColumn("dbo.ProductCategories", "ProductGroupId", c => c.Int());
            CreateIndex("dbo.ProductCategories", "ProductGroupId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductCategories", new[] { "ProductGroupId" });
            AlterColumn("dbo.ProductCategories", "ProductGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductCategories", "ProductGroupId");
        }
    }
}
