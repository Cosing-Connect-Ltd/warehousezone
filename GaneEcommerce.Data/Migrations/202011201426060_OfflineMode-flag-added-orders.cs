namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfflineModeflagaddedorders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OfflineSale", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OfflineSale");
        }
    }
}
