namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowInTopCategoryflagaddedinWebsiteNavigations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteNavigations", "ShowInTopCategory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteNavigations", "ShowInTopCategory");
        }
    }
}
