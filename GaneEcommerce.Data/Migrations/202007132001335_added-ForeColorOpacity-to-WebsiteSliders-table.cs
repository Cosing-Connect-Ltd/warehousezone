namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addedForeColorOpacitytoWebsiteSliderstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteSliders", "ForeColorOpacity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.WebsiteSliders", "ForeColorOpacity");
        }
    }
}
