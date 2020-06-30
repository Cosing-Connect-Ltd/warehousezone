namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addeddescriptiontoWebsiteShippingRulestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteShippingRules", "Description", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.WebsiteShippingRules", "Description");
        }
    }
}
