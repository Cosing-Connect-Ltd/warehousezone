namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsCollectionAvailableandIsDeliveryAvailablefieldstotenantwebsite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "IsCollectionAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantWebsites", "IsDeliveryAvailable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "IsDeliveryAvailable");
            DropColumn("dbo.TenantWebsites", "IsCollectionAvailable");
        }
    }
}
