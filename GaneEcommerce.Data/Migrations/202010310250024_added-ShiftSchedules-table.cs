namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedShiftSchedulestable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShiftSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Subject = c.String(),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        RecurrenceInfo = c.String(),
                        ResourceId = c.Int(nullable: false),
                        TenantId = c.Int(),
                        TimeBreaks = c.Time(precision: 7),
                        LocationsId = c.Int(),
                        IsCanceled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShiftSchedules", "TenantId", "dbo.Tenants");
            DropIndex("dbo.ShiftSchedules", new[] { "TenantId" });
            DropTable("dbo.ShiftSchedules");
        }
    }
}
