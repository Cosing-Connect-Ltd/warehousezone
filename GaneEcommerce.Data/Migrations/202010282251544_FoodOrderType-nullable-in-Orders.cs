namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodOrderTypenullableinOrders : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "FoodOrderType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "FoodOrderType", c => c.Int(nullable: false));
        }
    }
}
