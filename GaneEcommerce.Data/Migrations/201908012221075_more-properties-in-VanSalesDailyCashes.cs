namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morepropertiesinVanSalesDailyCashes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VanSalesDailyCashes", "TotalDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "TotalChequeSubmitted", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "TotalCardSubmitted", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "CashCount", c => c.Int(nullable: false));
            AddColumn("dbo.VanSalesDailyCashes", "CardCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSalesDailyCashes", "CardCount");
            DropColumn("dbo.VanSalesDailyCashes", "CashCount");
            DropColumn("dbo.VanSalesDailyCashes", "TotalCardSubmitted");
            DropColumn("dbo.VanSalesDailyCashes", "TotalChequeSubmitted");
            DropColumn("dbo.VanSalesDailyCashes", "TotalDiscount");
        }
    }
}
