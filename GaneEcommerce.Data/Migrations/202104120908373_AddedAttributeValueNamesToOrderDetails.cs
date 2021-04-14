namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAttributeValueNamesToOrderDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcessDetails", "ProductAttributeValueId", c => c.Int());
            CreateIndex("dbo.OrderProcessDetails", "ProductAttributeValueId");
            AddForeignKey("dbo.OrderProcessDetails", "ProductAttributeValueId", "dbo.ProductAttributeValues", "AttributeValueId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProcessDetails", "ProductAttributeValueId", "dbo.ProductAttributeValues");
            DropIndex("dbo.OrderProcessDetails", new[] { "ProductAttributeValueId" });
            DropColumn("dbo.OrderProcessDetails", "ProductAttributeValueId");
        }
    }
}
