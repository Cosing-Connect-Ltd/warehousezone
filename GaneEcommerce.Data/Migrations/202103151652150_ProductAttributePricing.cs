namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAttributePricing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributes", "IsPriced", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductAttributeValues", "AttributeSpecificPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAttributeValues", "AttributeSpecificPrice");
            DropColumn("dbo.ProductAttributes", "IsPriced");
        }
    }
}
