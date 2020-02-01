namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DirectShip_orders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DirectShip", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DirectShip");
        }
    }
}
