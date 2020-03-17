namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentLabelStatusadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletsDispatches", "LabelPrintStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletsDispatches", "LabelPrintStatus");
        }
    }
}
