namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobstartdateaddedinresources : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "JobStartDate", c => c.DateTime());
           
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "JobStartDate");
        }
    }
}
