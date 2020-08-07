namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class increase_value_field_size_in_ProductAttribiteValues_table : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductAttributeValues", "Value", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductAttributeValues", "Value", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
