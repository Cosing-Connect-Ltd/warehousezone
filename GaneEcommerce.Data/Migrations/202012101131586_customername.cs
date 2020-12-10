namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customername : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "CustomerName");
        }
    }
}
