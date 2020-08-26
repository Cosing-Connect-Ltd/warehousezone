namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_sort_order_to_ProductManufacturers_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductManufacturers", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductManufacturers", "SortOrder");
        }
    }
}
