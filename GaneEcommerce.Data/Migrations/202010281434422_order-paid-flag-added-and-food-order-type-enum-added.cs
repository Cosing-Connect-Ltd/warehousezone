namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderpaidflagaddedandfoodordertypeenumadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderPaid", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "FoodOrderType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "FoodOrderType");
            DropColumn("dbo.Orders", "OrderPaid");
        }
    }
}
