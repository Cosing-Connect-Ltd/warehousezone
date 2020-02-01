namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenantAgreement_TenantConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "TenantAgreement", c => c.String());
        }
           
        
        public override void Down()
        {
            
            DropColumn("dbo.TenantConfigs", "TenantAgreement");
        }
    }
}
