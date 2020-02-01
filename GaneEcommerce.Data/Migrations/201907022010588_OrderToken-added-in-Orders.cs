namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderTokenaddedinOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderToken", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderToken");
        }
    }
}
