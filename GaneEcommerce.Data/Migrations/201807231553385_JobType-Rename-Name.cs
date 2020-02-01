namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobTypeRenameName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobTypes", "Name", c => c.String(nullable: false));
            DropColumn("dbo.JobTypes", "Job_Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobTypes", "Job_Type", c => c.String(nullable: false));
            DropColumn("dbo.JobTypes", "Name");
        }
    }
}
