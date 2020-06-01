namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDefaultValuecolumntoUISettingItemstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UISettingItems", "DefaultValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UISettingItems", "DefaultValue");
        }
    }
}
