namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_ContactReceiverEmail_field_from_TenantWebsites_table : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TenantWebsites", "ContactReceiverEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantWebsites", "ContactReceiverEmail", c => c.String());
        }
    }
}
