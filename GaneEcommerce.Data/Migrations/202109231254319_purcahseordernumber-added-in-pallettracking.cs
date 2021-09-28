namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purcahseordernumberaddedinpallettracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletTrackings", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletTrackings", "OrderNumber");
        }
    }
}
