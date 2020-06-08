namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addingSelectorcolumntoUISettingItemstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UISettingItems", "Selector", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.UISettingItems", "Selector");
        }
    }
}
