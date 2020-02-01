namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stocktakeflagsforterminalsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "AllowStocktakeAddNew", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "AllowStocktakeEdit", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockTakeScanLogs", "RequestPalletSerial", c => c.String());
            AddColumn("dbo.TenantConfigs", "SessionTimeoutHours", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "SessionTimeoutHours");
            DropColumn("dbo.StockTakeScanLogs", "RequestPalletSerial");
            DropColumn("dbo.TenantLocations", "AllowStocktakeEdit");
            DropColumn("dbo.TenantLocations", "AllowStocktakeAddNew");
        }
    }
}
