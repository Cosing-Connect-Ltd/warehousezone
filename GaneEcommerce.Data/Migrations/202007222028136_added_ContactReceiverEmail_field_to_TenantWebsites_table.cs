namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ContactReceiverEmail_field_to_TenantWebsites_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "ContactReceiverEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "ContactReceiverEmail");
        }
    }
}
