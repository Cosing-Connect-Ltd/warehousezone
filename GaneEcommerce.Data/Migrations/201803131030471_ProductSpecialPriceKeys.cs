namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductSpecialPriceKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductSpecialPrices", "ProductID");
            CreateIndex("dbo.ProductSpecialPrices", "AccountID");
            AddForeignKey("dbo.ProductSpecialPrices", "AccountID", "dbo.Account", "AccountID");
            AddForeignKey("dbo.ProductSpecialPrices", "ProductID", "dbo.ProductMaster", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSpecialPrices", "ProductID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductSpecialPrices", "AccountID", "dbo.Account");
            DropIndex("dbo.ProductSpecialPrices", new[] { "AccountID" });
            DropIndex("dbo.ProductSpecialPrices", new[] { "ProductID" });
        }
    }
}
