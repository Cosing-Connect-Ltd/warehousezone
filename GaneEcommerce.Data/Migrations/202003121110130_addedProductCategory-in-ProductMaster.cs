namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedProductCategoryinProductMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "ProductCategoryId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "ProductCategoryId");
        }
    }
}
