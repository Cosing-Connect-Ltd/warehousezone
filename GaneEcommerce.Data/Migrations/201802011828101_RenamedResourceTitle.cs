namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedResourceTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "PersonTitle", c => c.Int());
            DropColumn("dbo.Resources", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Resources", "Title", c => c.Int());
            DropColumn("dbo.Resources", "PersonTitle");
        }
    }
}
