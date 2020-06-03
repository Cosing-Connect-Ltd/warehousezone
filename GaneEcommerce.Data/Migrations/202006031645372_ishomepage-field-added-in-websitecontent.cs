namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ishomepagefieldaddedinwebsitecontent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteContentPages", "IsHomePage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteContentPages", "IsHomePage");
        }
    }
}
