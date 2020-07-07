namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTextColorandForeColortoWebsiteSliderstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteSliders", "TextColor", c => c.String());
            AddColumn("dbo.WebsiteSliders", "ForeColor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteSliders", "ForeColor");
            DropColumn("dbo.WebsiteSliders", "TextColor");
        }
    }
}
