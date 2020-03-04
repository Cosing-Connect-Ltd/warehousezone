namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagepathintenantDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantDepartments", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantDepartments", "ImagePath");
        }
    }
}
