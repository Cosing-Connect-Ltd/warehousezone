namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleaddedAllowanceProduct : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductAllowances", "EmployeeRoles_Id", "dbo.EmployeeRoles");
            DropIndex("dbo.ProductAllowances", new[] { "EmployeeRoles_Id" });
            AlterColumn("dbo.WebsiteDiscountCodes", "FromDate", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.WebsiteDiscountCodes", "ToDate", c => c.DateTime(storeType: "date"));
            CreateIndex("dbo.ProductAllowances", "RolesId");
            AddForeignKey("dbo.ProductAllowances", "RolesId", "dbo.Roles", "Id");
            DropColumn("dbo.ProductAllowances", "EmployeeRoles_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductAllowances", "EmployeeRoles_Id", c => c.Int());
            DropForeignKey("dbo.ProductAllowances", "RolesId", "dbo.Roles");
            DropIndex("dbo.ProductAllowances", new[] { "RolesId" });
            AlterColumn("dbo.WebsiteDiscountCodes", "ToDate", c => c.DateTime());
            AlterColumn("dbo.WebsiteDiscountCodes", "FromDate", c => c.DateTime());
            CreateIndex("dbo.ProductAllowances", "EmployeeRoles_Id");
            AddForeignKey("dbo.ProductAllowances", "EmployeeRoles_Id", "dbo.EmployeeRoles", "Id");
        }
    }
}
