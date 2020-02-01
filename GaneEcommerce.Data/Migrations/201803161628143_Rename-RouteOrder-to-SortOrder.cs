namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRouteOrdertoSortOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MarketCustomers", "SortOrder", c => c.Int(nullable: false));
            DropColumn("dbo.MarketCustomers", "RouteOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MarketCustomers", "RouteOrder", c => c.Int(nullable: false));
            DropColumn("dbo.MarketCustomers", "SortOrder");
        }
    }
}
