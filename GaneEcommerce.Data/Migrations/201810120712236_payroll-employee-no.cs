namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollemployeeno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "PayrollEmployeeNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "PayrollEmployeeNo");
        }
    }
}
