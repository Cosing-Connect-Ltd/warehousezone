namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productattributeMaptableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAttributeMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        AttributeId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductAttributes", t => t.AttributeId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.AttributeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductAttributeMaps", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAttributeMaps", "AttributeId", "dbo.ProductAttributes");
            DropIndex("dbo.ProductAttributeMaps", new[] { "AttributeId" });
            DropIndex("dbo.ProductAttributeMaps", new[] { "ProductId" });
            DropTable("dbo.ProductAttributeMaps");
        }
    }
}
