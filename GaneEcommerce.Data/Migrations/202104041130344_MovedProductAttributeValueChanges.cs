namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedProductAttributeValueChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributeValuesMaps", "AttributeSpecificPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ProductAttributeValues", "AttributeSpecificPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductAttributeValues", "AttributeSpecificPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ProductAttributeValuesMaps", "AttributeSpecificPrice");
        }
    }
}
