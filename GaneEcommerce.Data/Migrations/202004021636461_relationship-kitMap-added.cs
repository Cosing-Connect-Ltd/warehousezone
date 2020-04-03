namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationshipkitMapadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductKitMaps", "ProductKitTypeId", c => c.Int());
            CreateIndex("dbo.ProductKitMaps", "ProductKitTypeId");
            AddForeignKey("dbo.ProductKitMaps", "ProductKitTypeId", "dbo.ProductKitTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductKitMaps", "ProductKitTypeId", "dbo.ProductKitTypes");
            DropIndex("dbo.ProductKitMaps", new[] { "ProductKitTypeId" });
            DropColumn("dbo.ProductKitMaps", "ProductKitTypeId");
        }
    }
}
