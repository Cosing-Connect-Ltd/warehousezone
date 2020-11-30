namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDeliverectrelatedfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "DeliverectChannel", c => c.String());
            AddColumn("dbo.TenantLocations", "DeliverectChannelLinkId", c => c.String());
            AddColumn("dbo.TenantLocations", "DeliverectChannelLinkName", c => c.String());
            AddColumn("dbo.ProductMaster", "DeliverectPLU", c => c.String());
            AddColumn("dbo.ProductMaster", "DeliverectProductType", c => c.Int(nullable: false));
            AddColumn("dbo.ProductMaster", "DeliverectProductId", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppOrderProcessType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "LoyaltyAppOrderProcessType");
            DropColumn("dbo.ProductMaster", "DeliverectProductId");
            DropColumn("dbo.ProductMaster", "DeliverectProductType");
            DropColumn("dbo.ProductMaster", "DeliverectPLU");
            DropColumn("dbo.TenantLocations", "DeliverectChannelLinkName");
            DropColumn("dbo.TenantLocations", "DeliverectChannelLinkId");
            DropColumn("dbo.TenantLocations", "DeliverectChannel");
        }
    }
}
