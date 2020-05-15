namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldssizeaddedtotooltipstable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tooltips", "Key", c => c.String(nullable: false, maxLength: 512));
            AlterColumn("dbo.Tooltips", "Title", c => c.String(maxLength: 1024));
            AlterColumn("dbo.Tooltips", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tooltips", "Description", c => c.String());
            AlterColumn("dbo.Tooltips", "Title", c => c.String());
            AlterColumn("dbo.Tooltips", "Key", c => c.String());
        }
    }
}
