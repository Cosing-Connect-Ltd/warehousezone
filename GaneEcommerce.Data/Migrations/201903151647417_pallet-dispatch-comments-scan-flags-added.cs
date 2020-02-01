namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class palletdispatchcommentsscanflagsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pallets", "ScannedOnLoading", c => c.Boolean(nullable: false));
            AddColumn("dbo.Pallets", "LoadingScanTime", c => c.DateTime());
            AddColumn("dbo.Pallets", "ScannedOnDelivered", c => c.Boolean(nullable: false));
            AddColumn("dbo.Pallets", "DeliveredScanTime", c => c.DateTime());
            AddColumn("dbo.PalletsDispatches", "ReceiverName", c => c.String());
            AddColumn("dbo.PalletsDispatches", "ReceiverSign", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletsDispatches", "ReceiverSign");
            DropColumn("dbo.PalletsDispatches", "ReceiverName");
            DropColumn("dbo.Pallets", "DeliveredScanTime");
            DropColumn("dbo.Pallets", "ScannedOnDelivered");
            DropColumn("dbo.Pallets", "LoadingScanTime");
            DropColumn("dbo.Pallets", "ScannedOnLoading");
        }
    }
}
