namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliverectCategoryIdandSortOrderadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "SortOrder", c => c.Int(nullable: false));
            AddColumn("dbo.TenantDepartments", "DeliverectCategoryId", c => c.String());
            AddColumn("dbo.TenantDepartments", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantDepartments", "SortOrder");
            DropColumn("dbo.TenantDepartments", "DeliverectCategoryId");
            DropColumn("dbo.ProductMaster", "SortOrder");
        }
    }
}
