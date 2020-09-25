namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_TenantDeliveryServiceId_to_orders_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TenantDeliveryServiceId", c => c.Int());
            CreateIndex("dbo.Orders", "TenantDeliveryServiceId");
            AddForeignKey("dbo.Orders", "TenantDeliveryServiceId", "dbo.TenantDeliveryServices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "TenantDeliveryServiceId", "dbo.TenantDeliveryServices");
            DropIndex("dbo.Orders", new[] { "TenantDeliveryServiceId" });
            DropColumn("dbo.Orders", "TenantDeliveryServiceId");
        }
    }
}
