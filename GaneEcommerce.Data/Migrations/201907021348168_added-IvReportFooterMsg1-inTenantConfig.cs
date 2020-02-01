namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIvReportFooterMsg1inTenantConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "IvReportFooterMsg1", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "IvReportFooterMsg1");
        }
    }
}
