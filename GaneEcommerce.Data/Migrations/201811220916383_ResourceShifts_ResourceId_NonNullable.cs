namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceShifts_ResourceId_NonNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ResourceShifts", new[] { "ResourceId" });
            AlterColumn("dbo.ResourceShifts", "ResourceId", c => c.Int(nullable: false));
            CreateIndex("dbo.ResourceShifts", "ResourceId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResourceShifts", new[] { "ResourceId" });
            AlterColumn("dbo.ResourceShifts", "ResourceId", c => c.Int());
            CreateIndex("dbo.ResourceShifts", "ResourceId");
        }
    }
}
