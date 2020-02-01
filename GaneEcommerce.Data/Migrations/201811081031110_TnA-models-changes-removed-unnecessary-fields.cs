namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TnAmodelschangesremovedunnecessaryfields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AttLogs_EmployeeShifts", "AttLogsId", "dbo.AttLogs");
            DropForeignKey("dbo.AttLogs_EmployeeShifts", "EmployeeShiftsId", "dbo.ResourceShifts");
            DropIndex("dbo.AttLogs_EmployeeShifts", new[] { "AttLogsId" });
            DropIndex("dbo.AttLogs_EmployeeShifts", new[] { "EmployeeShiftsId" });
            AlterColumn("dbo.ResourceShifts", "TimeStamp", c => c.DateTime(nullable: false));
            DropColumn("dbo.ResourceShifts", "StartTime");
            DropColumn("dbo.ResourceShifts", "EndTime");
            DropTable("dbo.AttLogs_EmployeeShifts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AttLogs_EmployeeShifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttLogsId = c.Int(nullable: false),
                        EmployeeShiftsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ResourceShifts", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResourceShifts", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ResourceShifts", "TimeStamp", c => c.DateTime());
            CreateIndex("dbo.AttLogs_EmployeeShifts", "EmployeeShiftsId");
            CreateIndex("dbo.AttLogs_EmployeeShifts", "AttLogsId");
            AddForeignKey("dbo.AttLogs_EmployeeShifts", "EmployeeShiftsId", "dbo.ResourceShifts", "Id");
            AddForeignKey("dbo.AttLogs_EmployeeShifts", "AttLogsId", "dbo.AttLogs", "Id");
        }
    }
}
