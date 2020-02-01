namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderIDForPProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PProperty", "Order_OrderID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PProperty", "Order_OrderID");
        }
    }
}
