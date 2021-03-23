namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductAttributeValueToOrderDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "ProductAttributeValueId", c => c.Int());
            CreateIndex("dbo.OrderDetails", "ProductAttributeValueId");
            AddForeignKey("dbo.OrderDetails", "ProductAttributeValueId", "dbo.ProductAttributeValues", "AttributeValueId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "ProductAttributeValueId", "dbo.ProductAttributeValues");
            DropIndex("dbo.OrderDetails", new[] { "ProductAttributeValueId" });
            DropColumn("dbo.OrderDetails", "ProductAttributeValueId");
        }
    }
}
