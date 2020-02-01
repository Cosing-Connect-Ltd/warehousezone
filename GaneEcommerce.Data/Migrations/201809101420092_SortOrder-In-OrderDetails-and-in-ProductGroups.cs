namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SortOrderInOrderDetailsandinProductGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "SortOrder", c => c.Int(nullable: false));
            AddColumn("dbo.ProductGroups", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductGroups", "SortOrder");
            DropColumn("dbo.OrderDetails", "SortOrder");
        }
    }
}
