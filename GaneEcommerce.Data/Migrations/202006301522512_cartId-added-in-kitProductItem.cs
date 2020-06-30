namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartIdaddedinkitProductItem : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.KitProductCartItems", new[] { "WebsiteCartItem_Id" });
            RenameColumn(table: "dbo.KitProductCartItems", name: "WebsiteCartItem_Id", newName: "CartId");
            AlterColumn("dbo.KitProductCartItems", "CartId", c => c.Int(nullable: false));
            CreateIndex("dbo.KitProductCartItems", "CartId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.KitProductCartItems", new[] { "CartId" });
            AlterColumn("dbo.KitProductCartItems", "CartId", c => c.Int());
            RenameColumn(table: "dbo.KitProductCartItems", name: "CartId", newName: "WebsiteCartItem_Id");
            CreateIndex("dbo.KitProductCartItems", "WebsiteCartItem_Id");
        }
    }
}
