namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingproductflageforecommerce : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "TopProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "BestSellerProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "SpecialProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "OnSaleProduct", c => c.Boolean(nullable: false));
           
        }
        
        public override void Down()
        {
            
            DropColumn("dbo.ProductMaster", "OnSaleProduct");
            DropColumn("dbo.ProductMaster", "SpecialProduct");
            DropColumn("dbo.ProductMaster", "BestSellerProduct");
            DropColumn("dbo.ProductMaster", "TopProduct");
        }
    }
}
