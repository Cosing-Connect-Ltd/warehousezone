namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHeadTenantInfoForPTenants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PTenant", "IsHeadTenant", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PTenant", "IsHeadTenant");
        }
    }
}
