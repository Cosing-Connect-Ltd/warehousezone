namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salebreakdownaddedinVanSalesDailyCashSync : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VanSalesDailyCashes", "TotalCashSale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "TotalCardSale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "TotalChequeSale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSalesDailyCashes", "TotalChequeSale");
            DropColumn("dbo.VanSalesDailyCashes", "TotalCardSale");
            DropColumn("dbo.VanSalesDailyCashes", "TotalCashSale");
        }
    }
}
