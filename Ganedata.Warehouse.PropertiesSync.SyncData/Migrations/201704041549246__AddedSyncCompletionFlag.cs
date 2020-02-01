namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedSyncCompletionFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PTenant", "SyncRequiredFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PTenant", "SyncRequiredFlag");
        }
    }
}
