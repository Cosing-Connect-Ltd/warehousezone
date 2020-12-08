namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewtimeandattendancerelatedfieldstoResourcestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "IsFlexibleWorkingAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Resources", "IsOvertimeWorkingAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Resources", "AttendanceGracePeriodInMinutes", c => c.Int(nullable: false));

            Sql("UPDATE dbo.Resources SET IsOvertimeWorkingAllowed = 1");
            Sql("UPDATE dbo.Resources SET IsFlexibleWorkingAllowed = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "AttendanceGracePeriodInMinutes");
            DropColumn("dbo.Resources", "IsOvertimeWorkingAllowed");
            DropColumn("dbo.Resources", "IsFlexibleWorkingAllowed");
        }
    }
}
