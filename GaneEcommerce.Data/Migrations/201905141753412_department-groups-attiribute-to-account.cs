namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class departmentgroupsattiributetoaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantDepartments", "AccountID", c => c.Int());
            AddColumn("dbo.OrderProcessDetails", "ID", c => c.String());
            AddColumn("dbo.ProductGroups", "DepartmentId", c => c.Int());
            CreateIndex("dbo.TenantDepartments", "AccountID");
            CreateIndex("dbo.ProductGroups", "DepartmentId");
            AddForeignKey("dbo.TenantDepartments", "AccountID", "dbo.Account", "AccountID");
            AddForeignKey("dbo.ProductGroups", "DepartmentId", "dbo.TenantDepartments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductGroups", "DepartmentId", "dbo.TenantDepartments");
            DropForeignKey("dbo.TenantDepartments", "AccountID", "dbo.Account");
            DropIndex("dbo.ProductGroups", new[] { "DepartmentId" });
            DropIndex("dbo.TenantDepartments", new[] { "AccountID" });
            DropColumn("dbo.ProductGroups", "DepartmentId");
            DropColumn("dbo.OrderProcessDetails", "ID");
            DropColumn("dbo.TenantDepartments", "AccountID");
        }
    }
}
