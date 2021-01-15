namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sortorderaddedinlocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "SortOrder", c => c.Int(nullable: false));
            AddColumn("dbo.Locations", "TenantId", c => c.Int(nullable: false));
            AlterColumn("dbo.Locations", "CreatedBy", c => c.Int());
            DropColumn("dbo.Locations", "TenentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "TenentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Locations", "CreatedBy", c => c.Int(nullable: false));
            DropColumn("dbo.Locations", "TenantId");
            DropColumn("dbo.Locations", "SortOrder");
        }
    }
}
