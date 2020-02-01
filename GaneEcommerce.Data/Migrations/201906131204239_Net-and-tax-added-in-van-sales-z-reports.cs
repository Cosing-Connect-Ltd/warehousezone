namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Netandtaxaddedinvansaleszreports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VanSalesDailyCashes", "TotalNetSale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.VanSalesDailyCashes", "TotalNetTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSalesDailyCashes", "TotalNetTax");
            DropColumn("dbo.VanSalesDailyCashes", "TotalNetSale");
        }
    }
}
