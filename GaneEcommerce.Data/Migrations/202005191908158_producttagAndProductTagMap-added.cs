namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class producttagAndProductTagMapadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductTagMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductTags", t => t.TagId)
                .Index(t => t.ProductId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.ProductTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                        SortOrder = c.Short(nullable: false),
                        IconImage = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.ProductMaster", "TopProduct");
            DropColumn("dbo.ProductMaster", "BestSellerProduct");
            DropColumn("dbo.ProductMaster", "SpecialProduct");
            DropColumn("dbo.ProductMaster", "OnSaleProduct");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductMaster", "OnSaleProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "SpecialProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "BestSellerProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "TopProduct", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.ProductTagMaps", "TagId", "dbo.ProductTags");
            DropForeignKey("dbo.ProductTagMaps", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductTagMaps", new[] { "TagId" });
            DropIndex("dbo.ProductTagMaps", new[] { "ProductId" });
            DropTable("dbo.ProductTags");
            DropTable("dbo.ProductTagMaps");
        }
    }
}
