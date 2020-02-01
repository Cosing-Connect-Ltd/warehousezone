namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commentsinpallettracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletTrackings", "Comments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletTrackings", "Comments");
        }
    }
}
