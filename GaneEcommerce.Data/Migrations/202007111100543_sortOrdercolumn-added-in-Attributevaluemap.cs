namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sortOrdercolumnaddedinAttributevaluemap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributeValues", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAttributeValues", "SortOrder");
        }
    }
}
