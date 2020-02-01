namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnsValuesPropertiesinVanSalesDailyCashes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductAccountCodes", new[] { "ProductId" });
            DropForeignKey("dbo.ProductAccountCodes", "ProductId", "dbo.ProductMaster");
            AddColumn("dbo.VanSalesDailyCashes", "ReturnOrderValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "ReturnOrderCount", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductAccountCodes", "ProductId", c => c.Int());
            CreateIndex("dbo.ProductAccountCodes", "ProductId");
            AddForeignKey("dbo.ProductAccountCodes", "ProductId", "dbo.ProductMaster", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductAccountCodes", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductAccountCodes", new[] { "ProductId" });
            AlterColumn("dbo.ProductAccountCodes", "ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.VanSalesDailyCashes", "ReturnOrderCount");
            DropColumn("dbo.VanSalesDailyCashes", "ReturnOrderValue");
            CreateIndex("dbo.ProductAccountCodes", "ProductId");
            AddForeignKey("dbo.ProductAccountCodes", "ProductId", "dbo.ProductMaster", "ProductId");
        }
    }
}
