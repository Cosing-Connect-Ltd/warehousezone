namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pickbycontainerpropertiesadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "PickByContainer", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "MandatoryPickByContainer", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderProcesses", "PickContainerCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "PickContainerCode");
            DropColumn("dbo.TenantLocations", "MandatoryPickByContainer");
            DropColumn("dbo.TenantLocations", "PickByContainer");
        }
    }
}
