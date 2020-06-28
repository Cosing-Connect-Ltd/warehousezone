namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class kitproductcartchildtableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KitProductCartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CartId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.WebsiteCartItems", t => t.CartId)
                .Index(t => t.ProductId)
                .Index(t => t.CartId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KitProductCartItems", "CartId", "dbo.WebsiteCartItems");
            DropForeignKey("dbo.KitProductCartItems", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.KitProductCartItems", new[] { "CartId" });
            DropIndex("dbo.KitProductCartItems", new[] { "ProductId" });
            DropTable("dbo.KitProductCartItems");
        }
    }
}
