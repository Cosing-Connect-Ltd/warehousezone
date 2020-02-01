namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantConfigTerminalreceiptprintproperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine1", c => c.String());
            AddColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine2", c => c.String());
            AddColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine3", c => c.String());
            AddColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine4", c => c.String());
            AddColumn("dbo.TenantConfigs", "TenantLogo", c => c.Binary());
            AddColumn("dbo.TenantConfigs", "PrintLogoForReceipts", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "PrintLogoForReceipts");
            DropColumn("dbo.TenantConfigs", "TenantLogo");
            DropColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine4");
            DropColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine3");
            DropColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine2");
            DropColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine1");
        }
    }
}
