namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PalletTrackingModelChnages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletTrackings", "RemainingCases", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PalletTrackings", "TotalCases", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PalletTrackings", "RemainingQuantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PalletTrackings", "RemainingQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PalletTrackings", "TotalCases");
            DropColumn("dbo.PalletTrackings", "RemainingCases");
        }
    }
}
