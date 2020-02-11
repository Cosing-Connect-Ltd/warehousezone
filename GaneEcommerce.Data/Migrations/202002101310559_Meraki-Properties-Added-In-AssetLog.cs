namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MerakiPropertiesAddedInAssetLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetLogs", "Ipv4", c => c.String());
            AddColumn("dbo.AssetLogs", "SeenTime", c => c.DateTime());
            AddColumn("dbo.AssetLogs", "SeenEpoch", c => c.Long(nullable: false));
            AddColumn("dbo.AssetLogs", "Ssid", c => c.String());
            AddColumn("dbo.AssetLogs", "Os", c => c.String());
            AddColumn("dbo.AssetLogs", "ClientMac", c => c.String());
            AddColumn("dbo.AssetLogs", "Manufacturer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetLogs", "Manufacturer");
            DropColumn("dbo.AssetLogs", "ClientMac");
            DropColumn("dbo.AssetLogs", "Os");
            DropColumn("dbo.AssetLogs", "Ssid");
            DropColumn("dbo.AssetLogs", "SeenEpoch");
            DropColumn("dbo.AssetLogs", "SeenTime");
            DropColumn("dbo.AssetLogs", "Ipv4");
        }
    }
}
