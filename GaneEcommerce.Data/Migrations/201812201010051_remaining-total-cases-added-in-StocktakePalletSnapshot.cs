namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remainingtotalcasesaddedinStocktakePalletSnapshot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockTakePalletsSnapshots", "RemainingCases", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StockTakePalletsSnapshots", "TotalCases", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockTakePalletsSnapshots", "TotalCases");
            DropColumn("dbo.StockTakePalletsSnapshots", "RemainingCases");
        }
    }
}
