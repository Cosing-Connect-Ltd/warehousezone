namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setuseridnullableinCartitems : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebsiteCartItems", new[] { "UserId" });
            AlterColumn("dbo.WebsiteCartItems", "UserId", c => c.Int());
            CreateIndex("dbo.WebsiteCartItems", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WebsiteCartItems", new[] { "UserId" });
            AlterColumn("dbo.WebsiteCartItems", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.WebsiteCartItems", "UserId");
        }
    }
}
