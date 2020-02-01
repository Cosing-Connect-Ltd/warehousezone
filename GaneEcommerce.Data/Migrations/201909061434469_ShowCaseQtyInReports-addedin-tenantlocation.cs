namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowCaseQtyInReportsaddedintenantlocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "ShowCaseQtyInReports", c => c.Boolean(nullable: false));

        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "ShowCaseQtyInReports");
        }
    }
}
