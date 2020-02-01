namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedSyncHistoryEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PSyncHistory",
                c => new
                    {
                        PSyncHistoryId = c.Int(nullable: false, identity: true),
                        SyncTime = c.DateTime(nullable: false),
                        TenantsSynced = c.Int(nullable: false),
                        LandlordsSynced = c.Int(nullable: false),
                        PropertiesSynced = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PSyncHistoryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PSyncHistory");
        }
    }
}
