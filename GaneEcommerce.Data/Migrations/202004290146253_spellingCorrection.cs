namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spellingCorrection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteNavigations", "ImageAltTag", c => c.String());
            AddColumn("dbo.WebsiteNavigations", "HoverImageAltTag", c => c.String());
            AddColumn("dbo.WebsiteSliders", "ImageAltTag", c => c.String());
            DropColumn("dbo.WebsiteNavigations", "IamgeAltTag");
            DropColumn("dbo.WebsiteNavigations", "HoverIamgeAltTag");
            DropColumn("dbo.WebsiteSliders", "IamgeAltTag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebsiteSliders", "IamgeAltTag", c => c.String());
            AddColumn("dbo.WebsiteNavigations", "HoverIamgeAltTag", c => c.String());
            AddColumn("dbo.WebsiteNavigations", "IamgeAltTag", c => c.String());
            DropColumn("dbo.WebsiteSliders", "ImageAltTag");
            DropColumn("dbo.WebsiteNavigations", "HoverImageAltTag");
            DropColumn("dbo.WebsiteNavigations", "ImageAltTag");
        }
    }
}
