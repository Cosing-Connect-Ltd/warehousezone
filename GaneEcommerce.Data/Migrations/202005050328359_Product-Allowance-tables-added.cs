namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAllowancetablesadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAllowances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RolesId = c.Int(nullable: false),
                        PerXDays = c.Int(nullable: false),
                        ProductId = c.Int(),
                        AllowanceGroupId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        EmployeeRoles_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeRoles", t => t.EmployeeRoles_Id)
                .ForeignKey("dbo.ProductAllowanceGroups", t => t.AllowanceGroupId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.ProductId)
                .Index(t => t.AllowanceGroupId)
                .Index(t => t.TenantId)
                .Index(t => t.EmployeeRoles_Id);
            
            CreateTable(
                "dbo.ProductAllowanceAdjustmentLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AllowanceId = c.Int(nullable: false),
                        Reason = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductAllowances", t => t.AllowanceId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.AllowanceId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ProductAllowanceGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Notes = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ProductAllowanceGroupMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        AllowanceGroupId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductAllowanceGroups", t => t.AllowanceGroupId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.AllowanceGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductAllowances", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductAllowances", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAllowances", "AllowanceGroupId", "dbo.ProductAllowanceGroups");
            DropForeignKey("dbo.ProductAllowanceGroups", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductAllowanceGroupMaps", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAllowanceGroupMaps", "AllowanceGroupId", "dbo.ProductAllowanceGroups");
            DropForeignKey("dbo.ProductAllowanceAdjustmentLogs", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductAllowanceAdjustmentLogs", "AllowanceId", "dbo.ProductAllowances");
            DropForeignKey("dbo.ProductAllowances", "EmployeeRoles_Id", "dbo.EmployeeRoles");
            DropIndex("dbo.ProductAllowanceGroupMaps", new[] { "AllowanceGroupId" });
            DropIndex("dbo.ProductAllowanceGroupMaps", new[] { "ProductId" });
            DropIndex("dbo.ProductAllowanceGroups", new[] { "TenantId" });
            DropIndex("dbo.ProductAllowanceAdjustmentLogs", new[] { "TenantId" });
            DropIndex("dbo.ProductAllowanceAdjustmentLogs", new[] { "AllowanceId" });
            DropIndex("dbo.ProductAllowances", new[] { "EmployeeRoles_Id" });
            DropIndex("dbo.ProductAllowances", new[] { "TenantId" });
            DropIndex("dbo.ProductAllowances", new[] { "AllowanceGroupId" });
            DropIndex("dbo.ProductAllowances", new[] { "ProductId" });
            DropTable("dbo.ProductAllowanceGroupMaps");
            DropTable("dbo.ProductAllowanceGroups");
            DropTable("dbo.ProductAllowanceAdjustmentLogs");
            DropTable("dbo.ProductAllowances");
        }
    }
}
