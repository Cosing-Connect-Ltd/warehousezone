namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableallowanceGroupId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductAllowances", new[] { "AllowanceGroupId" });
            AlterColumn("dbo.ProductAllowances", "AllowanceGroupId", c => c.Int());
            CreateIndex("dbo.ProductAllowances", "AllowanceGroupId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductAllowances", new[] { "AllowanceGroupId" });
            AlterColumn("dbo.ProductAllowances", "AllowanceGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductAllowances", "AllowanceGroupId");
        }
    }
}
