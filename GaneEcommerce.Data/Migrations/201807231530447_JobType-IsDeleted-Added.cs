namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobTypeIsDeletedAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobTypes", "DateUpdated", c => c.DateTime());
            AddColumn("dbo.JobTypes", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.JobTypes", "CreatedBy", c => c.Int());
            DropColumn("dbo.JobTypes", "DateUpdate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobTypes", "DateUpdate", c => c.DateTime());
            AlterColumn("dbo.JobTypes", "CreatedBy", c => c.Int(nullable: false));
            DropColumn("dbo.JobTypes", "IsDeleted");
            DropColumn("dbo.JobTypes", "DateUpdated");
        }
    }
}
