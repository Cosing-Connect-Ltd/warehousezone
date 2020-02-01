namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Locations_Type_and_Group_nullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Locations", new[] { "LocationGroupId" });
            DropIndex("dbo.Locations", new[] { "LocationTypeId" });
            AlterColumn("dbo.Locations", "LocationGroupId", c => c.Int());
            AlterColumn("dbo.Locations", "LocationTypeId", c => c.Int());
            CreateIndex("dbo.Locations", "LocationGroupId");
            CreateIndex("dbo.Locations", "LocationTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Locations", new[] { "LocationTypeId" });
            DropIndex("dbo.Locations", new[] { "LocationGroupId" });
            AlterColumn("dbo.Locations", "LocationTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Locations", "LocationGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Locations", "LocationTypeId");
            CreateIndex("dbo.Locations", "LocationGroupId");
        }
    }
}
