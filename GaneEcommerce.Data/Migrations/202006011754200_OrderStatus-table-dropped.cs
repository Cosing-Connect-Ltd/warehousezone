namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderStatustabledropped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "OrderStatusID", "dbo.OrderStatus");
            DropForeignKey("dbo.OrderDetails", "OrderDetailStatusId", "dbo.OrderStatus");
            DropIndex("dbo.OrderDetails", new[] { "OrderDetailStatusId" });
            DropIndex("dbo.Orders", new[] { "OrderStatusID" });
            DropTable("dbo.OrderStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        OrderStatusID = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.OrderStatusID);
            
            CreateIndex("dbo.Orders", "OrderStatusID");
            CreateIndex("dbo.OrderDetails", "OrderDetailStatusId");
            AddForeignKey("dbo.OrderDetails", "OrderDetailStatusId", "dbo.OrderStatus", "OrderStatusID");
            AddForeignKey("dbo.Orders", "OrderStatusID", "dbo.OrderStatus", "OrderStatusID");
        }
    }
}
