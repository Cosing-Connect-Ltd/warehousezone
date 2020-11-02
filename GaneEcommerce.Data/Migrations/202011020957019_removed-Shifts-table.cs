namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedShiftstable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shifts", "Resources_ResourceId", "dbo.Resources");
            DropForeignKey("dbo.Shifts", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Shifts", "TenantLocations_WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.ShiftSchedules", "TenantId", "dbo.Tenants");
            DropIndex("dbo.Shifts", new[] { "TenantId" });
            DropIndex("dbo.Shifts", new[] { "Resources_ResourceId" });
            DropIndex("dbo.Shifts", new[] { "TenantLocations_WarehouseId" });
            DropIndex("dbo.ShiftSchedules", new[] { "TenantId" });
            CreateIndex("dbo.ShiftSchedules", "ResourceId");
            AddForeignKey("dbo.ShiftSchedules", "ResourceId", "dbo.Resources", "ResourceId");
            DropTable("dbo.Shifts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        WeekNumber = c.Int(),
                        WeekDay = c.Int(),
                        ExpectedHours = c.Time(precision: 7),
                        TimeBreaks = c.Time(precision: 7),
                        LocationsId = c.Int(),
                        Date = c.DateTime(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        Resources_ResourceId = c.Int(),
                        TenantLocations_WarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.ShiftSchedules", "ResourceId", "dbo.Resources");
            DropIndex("dbo.ShiftSchedules", new[] { "ResourceId" });
            CreateIndex("dbo.ShiftSchedules", "TenantId");
            CreateIndex("dbo.Shifts", "TenantLocations_WarehouseId");
            CreateIndex("dbo.Shifts", "Resources_ResourceId");
            CreateIndex("dbo.Shifts", "TenantId");
            AddForeignKey("dbo.ShiftSchedules", "TenantId", "dbo.Tenants", "TenantId");
            AddForeignKey("dbo.Shifts", "TenantLocations_WarehouseId", "dbo.TenantLocations", "WarehouseId");
            AddForeignKey("dbo.Shifts", "TenantId", "dbo.Tenants", "TenantId");
            AddForeignKey("dbo.Shifts", "Resources_ResourceId", "dbo.Resources", "ResourceId");
        }
    }
}
