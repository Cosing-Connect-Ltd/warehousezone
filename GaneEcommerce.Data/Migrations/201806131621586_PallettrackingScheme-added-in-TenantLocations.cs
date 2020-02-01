namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PallettrackingSchemeaddedinTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "PalletTrackingScheme", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "PalletTrackingScheme");
        }
    }
}
