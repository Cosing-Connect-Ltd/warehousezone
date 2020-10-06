namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBaseOrderIdtoOrderstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "BaseOrderID", c => c.Int());
            CreateIndex("dbo.Orders", "BaseOrderID");
            AddForeignKey("dbo.Orders", "BaseOrderID", "dbo.Orders", "OrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "BaseOrderID", "dbo.Orders");
            DropIndex("dbo.Orders", new[] { "BaseOrderID" });
            DropColumn("dbo.Orders", "BaseOrderID");
        }
    }
}
