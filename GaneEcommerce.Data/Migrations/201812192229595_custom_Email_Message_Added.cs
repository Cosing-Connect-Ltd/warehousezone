namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom_Email_Message_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantEmailNotificationQueues", "CustomEmailMessage", c => c.String());
           
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantEmailNotificationQueues", "CustomEmailMessage");
        }
    }
}
