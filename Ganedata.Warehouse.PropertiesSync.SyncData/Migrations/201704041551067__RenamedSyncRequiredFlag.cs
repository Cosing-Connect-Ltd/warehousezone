namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _RenamedSyncRequiredFlag : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.PTenant", "SyncRequiredFlag", c => c.Boolean(nullable: false));
            //DropColumn("dbo.PTenant", "ChangeSynced");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.PTenant", "ChangeSynced", c => c.Boolean(nullable: false));
            //DropColumn("dbo.PTenant", "SyncRequiredFlag");
        }
    }
}
