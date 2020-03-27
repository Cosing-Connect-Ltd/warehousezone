namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupedproductadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "GroupedProduct", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "GroupedProduct");
        }
    }
}
