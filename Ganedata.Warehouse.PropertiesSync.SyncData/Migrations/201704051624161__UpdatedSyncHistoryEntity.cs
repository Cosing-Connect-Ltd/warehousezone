namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _UpdatedSyncHistoryEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PSyncHistory", "SyncStartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.PSyncHistory", "ImportCompletedTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.PSyncHistory", "SyncCompletedTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.PSyncHistory", "SyncTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PSyncHistory", "SyncTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.PSyncHistory", "SyncCompletedTime");
            DropColumn("dbo.PSyncHistory", "ImportCompletedTime");
            DropColumn("dbo.PSyncHistory", "SyncStartTime");
        }
    }
}
