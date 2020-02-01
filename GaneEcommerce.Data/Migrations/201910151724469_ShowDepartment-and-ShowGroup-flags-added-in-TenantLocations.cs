namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowDepartmentandShowGroupflagsaddedinTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "ShowDepartmentInBlindShipment", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "ShowGroupInBlindShipment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "ShowGroupInBlindShipment");
            DropColumn("dbo.TenantLocations", "ShowDepartmentInBlindShipment");
        }
    }
}
