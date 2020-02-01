namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timeaddedinexpecteddate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "ExpectedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "ExpectedDate", c => c.DateTime(storeType: "date"));
        }
    }
}
