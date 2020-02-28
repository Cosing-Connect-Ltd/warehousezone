namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountNumberinGlobalApi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GlobalApis", "AccountNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GlobalApis", "AccountNumber");
        }
    }
}
