namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingEmailChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebsiteWishListItems", new[] { "UserId" });
            AlterColumn("dbo.WebsiteWishListItems", "UserId", c => c.Int());
            CreateIndex("dbo.WebsiteWishListItems", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WebsiteWishListItems", new[] { "UserId" });
            AlterColumn("dbo.WebsiteWishListItems", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.WebsiteWishListItems", "UserId");
        }
    }
}
