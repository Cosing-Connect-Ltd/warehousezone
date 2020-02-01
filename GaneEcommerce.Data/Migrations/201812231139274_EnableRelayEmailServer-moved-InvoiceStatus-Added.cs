namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableRelayEmailServermovedInvoiceStatusAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantEmailConfigs", "EnableRelayEmailServer", c => c.Boolean());
            AddColumn("dbo.InvoiceMaster", "InvoiceStatus", c => c.Int(nullable: false));
            DropColumn("dbo.TenantConfigs", "EnabledRelayEmailServer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantConfigs", "EnabledRelayEmailServer", c => c.Boolean());
            DropColumn("dbo.InvoiceMaster", "InvoiceStatus");
            DropColumn("dbo.TenantEmailConfigs", "EnableRelayEmailServer");
        }
    }
}
