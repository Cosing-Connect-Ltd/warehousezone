namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredfieldaddedforwebsitecontentandnavigation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WebsiteNavigations", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.WebsiteContentPages", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WebsiteContentPages", "Title", c => c.String());
            AlterColumn("dbo.WebsiteNavigations", "Name", c => c.String());
        }
    }
}
