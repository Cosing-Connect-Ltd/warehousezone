namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenantconfigreportfootermessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "PoReportFooterMsg1", c => c.String());
            AddColumn("dbo.TenantConfigs", "PoReportFooterMsg2", c => c.String());
            AddColumn("dbo.TenantConfigs", "SoReportFooterMsg1", c => c.String());
            AddColumn("dbo.TenantConfigs", "SoReportFooterMsg2", c => c.String());
            AddColumn("dbo.TenantConfigs", "DnReportFooterMsg1", c => c.String());
            AddColumn("dbo.TenantConfigs", "DnReportFooterMsg2", c => c.String());
            DropColumn("dbo.TenantConfigs", "PurchaseOrderReportFooter");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantConfigs", "PurchaseOrderReportFooter", c => c.String());
            DropColumn("dbo.TenantConfigs", "DnReportFooterMsg2");
            DropColumn("dbo.TenantConfigs", "DnReportFooterMsg1");
            DropColumn("dbo.TenantConfigs", "SoReportFooterMsg2");
            DropColumn("dbo.TenantConfigs", "SoReportFooterMsg1");
            DropColumn("dbo.TenantConfigs", "PoReportFooterMsg2");
            DropColumn("dbo.TenantConfigs", "PoReportFooterMsg1");
        }
    }
}
