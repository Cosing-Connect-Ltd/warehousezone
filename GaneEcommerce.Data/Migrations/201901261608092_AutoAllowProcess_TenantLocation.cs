namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoAllowProcess_TenantLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "AutoAllowProcess", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "AutoAllowProcess");
        }
    }
}
