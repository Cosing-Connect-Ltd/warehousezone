namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotesfieldaddedinOrderProcessDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcessDetails", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcessDetails", "Notes");
        }
    }
}
