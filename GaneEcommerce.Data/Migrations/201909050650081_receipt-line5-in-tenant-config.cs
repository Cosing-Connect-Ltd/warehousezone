namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receiptline5intenantconfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine5", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "TenantReceiptPrintHeaderLine5");
        }
    }
}
