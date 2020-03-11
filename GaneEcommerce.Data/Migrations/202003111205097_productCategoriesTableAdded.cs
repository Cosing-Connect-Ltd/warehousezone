namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productCategoriesTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        ProductCategoryId = c.Int(nullable: false, identity: true),
                        ProductCategoryName = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        ProductGroupId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductCategoryId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId)
                .Index(t => t.ProductGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductCategories", "ProductGroupId", "dbo.ProductGroups");
            DropIndex("dbo.ProductCategories", new[] { "ProductGroupId" });
            DropTable("dbo.ProductCategories");
        }
    }
}
