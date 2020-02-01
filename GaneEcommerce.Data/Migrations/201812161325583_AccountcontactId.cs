namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountcontactId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderPTenantEmailRecipients", "AccountContactId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderPTenantEmailRecipients", "AccountContactId");
        }
    }
}
