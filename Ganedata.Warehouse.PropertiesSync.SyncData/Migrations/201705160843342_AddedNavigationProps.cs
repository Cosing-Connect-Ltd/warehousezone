namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNavigationProps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PProperty", "CurrentTenantCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PProperty", "CurrentTenantCode");
        }
    }
}
