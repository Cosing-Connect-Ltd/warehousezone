namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pendingchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShipmentAccountAddressId", c => c.Int());
            AddColumn("dbo.Orders", "ConsignmentTypeId", c => c.Int());
            CreateIndex("dbo.Orders", "ShipmentAccountAddressId");
            CreateIndex("dbo.Orders", "ConsignmentTypeId");
            AddForeignKey("dbo.Orders", "ConsignmentTypeId", "dbo.OrderConsignmentTypes", "ConsignmentTypeId");
            AddForeignKey("dbo.Orders", "ShipmentAccountAddressId", "dbo.AccountAddresses", "AddressID");
            DropColumn("dbo.OrderProcessStatus", "TenantId");
            DropColumn("dbo.OrderProcessStatus", "DateCreated");
            DropColumn("dbo.OrderProcessStatus", "DateUpdated");
            DropColumn("dbo.OrderProcessStatus", "CreatedBy");
            DropColumn("dbo.OrderProcessStatus", "UpdatedBy");
            DropColumn("dbo.OrderProcessStatus", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProcessStatus", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.OrderProcessStatus", "UpdatedBy", c => c.Int());
            AddColumn("dbo.OrderProcessStatus", "CreatedBy", c => c.Int());
            AddColumn("dbo.OrderProcessStatus", "DateUpdated", c => c.DateTime());
            AddColumn("dbo.OrderProcessStatus", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.OrderProcessStatus", "TenantId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "ShipmentAccountAddressId", "dbo.AccountAddresses");
            DropForeignKey("dbo.Orders", "ConsignmentTypeId", "dbo.OrderConsignmentTypes");
            DropIndex("dbo.Orders", new[] { "ConsignmentTypeId" });
            DropIndex("dbo.Orders", new[] { "ShipmentAccountAddressId" });
            DropColumn("dbo.Orders", "ConsignmentTypeId");
            DropColumn("dbo.Orders", "ShipmentAccountAddressId");
        }
    }
}
