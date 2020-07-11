namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sortorderolumnaddedinproductarrtibutes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributes", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAttributes", "SortOrder");
        }
    }
}
