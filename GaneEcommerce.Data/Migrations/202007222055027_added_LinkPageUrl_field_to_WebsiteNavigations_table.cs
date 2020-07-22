namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_LinkPageUrl_field_to_WebsiteNavigations_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteNavigations", "LinkPageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteNavigations", "LinkPageUrl");
        }
    }
}
