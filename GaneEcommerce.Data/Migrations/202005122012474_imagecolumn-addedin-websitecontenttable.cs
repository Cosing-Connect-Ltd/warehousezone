namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagecolumnaddedinwebsitecontenttable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteContentPages", "ShortDescription", c => c.String());
            AddColumn("dbo.WebsiteContentPages", "Image", c => c.String());
            AddColumn("dbo.WebsiteContentPages", "ImageAltTag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteContentPages", "ImageAltTag");
            DropColumn("dbo.WebsiteContentPages", "Image");
            DropColumn("dbo.WebsiteContentPages", "ShortDescription");
        }
    }
}
