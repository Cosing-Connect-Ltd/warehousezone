namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replace_ConsignmentType_with_DeliveryMethod_everywhere : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "ConsignmentTypeId", "dbo.OrderConsignmentTypes");
            DropForeignKey("dbo.OrderProcesses", "ConsignmentTypeId", "dbo.OrderConsignmentTypes");
            DropForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.OrderConsignmentTypes");
            DropIndex("dbo.Orders", new[] { "ConsignmentTypeId" });
            DropIndex("dbo.OrderProcesses", new[] { "ConsignmentTypeId" });
            DropIndex("dbo.TenantWarranties", new[] { "PostageTypeId" });
            AddColumn("dbo.Orders", "DeliveryMethod", c => c.Int());
            AddColumn("dbo.OrderProcesses", "DeliveryMethod", c => c.Int());
            AddColumn("dbo.ProductSerialis", "DeliveryMethod", c => c.Int());
            AddColumn("dbo.TenantWarranties", "DeliveryMethod", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "ConsignmentTypeId");
            DropColumn("dbo.OrderProcesses", "ConsignmentTypeId");
            DropColumn("dbo.ProductSerialis", "PostageTypeId");
            DropColumn("dbo.TenantWarranties", "PostageTypeId");
            DropTable("dbo.OrderConsignmentTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrderConsignmentTypes",
                c => new
                    {
                        ConsignmentTypeId = c.Int(nullable: false, identity: true),
                        ConsignmentType = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ConsignmentTypeId);
            
            AddColumn("dbo.TenantWarranties", "PostageTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductSerialis", "PostageTypeId", c => c.Int());
            AddColumn("dbo.OrderProcesses", "ConsignmentTypeId", c => c.Int());
            AddColumn("dbo.Orders", "ConsignmentTypeId", c => c.Int());
            DropColumn("dbo.TenantWarranties", "DeliveryMethod");
            DropColumn("dbo.ProductSerialis", "DeliveryMethod");
            DropColumn("dbo.OrderProcesses", "DeliveryMethod");
            DropColumn("dbo.Orders", "DeliveryMethod");
            CreateIndex("dbo.TenantWarranties", "PostageTypeId");
            CreateIndex("dbo.OrderProcesses", "ConsignmentTypeId");
            CreateIndex("dbo.Orders", "ConsignmentTypeId");
            AddForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.OrderConsignmentTypes", "ConsignmentTypeId");
            AddForeignKey("dbo.OrderProcesses", "ConsignmentTypeId", "dbo.OrderConsignmentTypes", "ConsignmentTypeId");
            AddForeignKey("dbo.Orders", "ConsignmentTypeId", "dbo.OrderConsignmentTypes", "ConsignmentTypeId");
        }
    }
}
