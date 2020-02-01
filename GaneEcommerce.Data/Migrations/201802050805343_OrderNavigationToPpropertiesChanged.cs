namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderNavigationToPpropertiesChanged : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PProperties", new[] { "Order_OrderID" });
            DropForeignKey("dbo.PProperties", "FK_dbo.PProperties_dbo.Orders_Order_OrderID");
            AddColumn("dbo.Orders", "PProperties_PPropertyId", c => c.Int());
            //RenameColumn(table: "dbo.Orders", name: "Order_OrderID", newName: "PProperties_PPropertyId");
            CreateIndex("dbo.Orders", "PProperties_PPropertyId");
            DropColumn("dbo.PProperties", "Order_OrderID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PProperties", "Order_OrderID", c => c.Int());
            DropIndex("dbo.Orders", new[] { "PProperties_PPropertyId" });
            DropColumn("dbo.Orders", "PProperties_PPropertyId");
            AddForeignKey("dbo.PProperties", "Order_OrderID", "dbo.Orders", "OrderID");
            //RenameColumn(table: "dbo.Orders", name: "PProperties_PPropertyId", newName: "Order_OrderID");
            CreateIndex("dbo.PProperties", "Order_OrderID");
        }
    }
}
