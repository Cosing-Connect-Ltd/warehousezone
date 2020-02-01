namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SiteIdaddedinordertable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "SiteID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "SiteID");
        }
    }
}
