namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactPageUrladdedinTenantWebsites : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "ContactPageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "ContactPageUrl");
        }
    }
}
