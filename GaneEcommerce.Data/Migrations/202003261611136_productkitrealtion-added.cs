namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productkitrealtionadded : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductKitMaps", new[] { "KitProductMaster_ProductId" });
            DropColumn("dbo.ProductKitMaps", "KitProductId");
            RenameColumn(table: "dbo.ProductKitMaps", name: "KitProductMaster_ProductId", newName: "KitProductId");
            AlterColumn("dbo.ProductKitMaps", "KitProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductKitMaps", "KitProductId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductKitMaps", new[] { "KitProductId" });
            AlterColumn("dbo.ProductKitMaps", "KitProductId", c => c.Int());
            RenameColumn(table: "dbo.ProductKitMaps", name: "KitProductId", newName: "KitProductMaster_ProductId");
            AddColumn("dbo.ProductKitMaps", "KitProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductKitMaps", "KitProductMaster_ProductId");
        }
    }
}
