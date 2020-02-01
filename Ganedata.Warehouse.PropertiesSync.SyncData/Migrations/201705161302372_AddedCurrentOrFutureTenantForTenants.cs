namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCurrentOrFutureTenantForTenants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PTenant", "IsCurrentTenant", c => c.Boolean(nullable: false));
            AddColumn("dbo.PTenant", "IsFutureTenant", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PTenant", "IsFutureTenant");
            DropColumn("dbo.PTenant", "IsCurrentTenant");
        }
    }
}
