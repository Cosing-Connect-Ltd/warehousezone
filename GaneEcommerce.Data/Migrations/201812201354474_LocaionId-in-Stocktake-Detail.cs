namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocaionIdinStocktakeDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockTakeDetails", "LocationId", c => c.Int());
            CreateIndex("dbo.StockTakeDetails", "LocationId");
            AddForeignKey("dbo.StockTakeDetails", "LocationId", "dbo.Locations", "LocationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockTakeDetails", "LocationId", "dbo.Locations");
            DropIndex("dbo.StockTakeDetails", new[] { "LocationId" });
            DropColumn("dbo.StockTakeDetails", "LocationId");
        }
    }
}
