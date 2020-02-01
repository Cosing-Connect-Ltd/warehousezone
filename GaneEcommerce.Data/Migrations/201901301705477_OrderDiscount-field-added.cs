namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDiscountfieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StockTakeDetails", "BatchNumber", c => c.String());
            AddColumn("dbo.StockTakeDetails", "ExpiryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockTakeDetails", "ExpiryDate");
            DropColumn("dbo.StockTakeDetails", "BatchNumber");
            DropColumn("dbo.Orders", "OrderDiscount");
        }
    }
}
