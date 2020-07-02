namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sessionkeyaddedinwebsitecartitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteCartItems", "SessionKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteCartItems", "SessionKey");
        }
    }
}
