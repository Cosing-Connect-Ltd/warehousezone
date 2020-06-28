namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingmainproductinkitproductcartitemstable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.KitProductCartItems", new[] { "CartId" });
            RenameColumn(table: "dbo.KitProductCartItems", name: "CartId", newName: "WebsiteCartItem_Id");
            RenameColumn(table: "dbo.KitProductCartItems", name: "ProductId", newName: "KitProductId");
            RenameIndex(table: "dbo.KitProductCartItems", name: "IX_ProductId", newName: "IX_KitProductId");
            AddColumn("dbo.KitProductCartItems", "SimpleProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.KitProductCartItems", "WebsiteCartItem_Id", c => c.Int());
            CreateIndex("dbo.KitProductCartItems", "SimpleProductId");
            CreateIndex("dbo.KitProductCartItems", "WebsiteCartItem_Id");
            AddForeignKey("dbo.KitProductCartItems", "SimpleProductId", "dbo.ProductMaster", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KitProductCartItems", "SimpleProductId", "dbo.ProductMaster");
            DropIndex("dbo.KitProductCartItems", new[] { "WebsiteCartItem_Id" });
            DropIndex("dbo.KitProductCartItems", new[] { "SimpleProductId" });
            AlterColumn("dbo.KitProductCartItems", "WebsiteCartItem_Id", c => c.Int(nullable: false));
            DropColumn("dbo.KitProductCartItems", "SimpleProductId");
            RenameIndex(table: "dbo.KitProductCartItems", name: "IX_KitProductId", newName: "IX_ProductId");
            RenameColumn(table: "dbo.KitProductCartItems", name: "KitProductId", newName: "ProductId");
            RenameColumn(table: "dbo.KitProductCartItems", name: "WebsiteCartItem_Id", newName: "CartId");
            CreateIndex("dbo.KitProductCartItems", "CartId");
        }
    }
}
