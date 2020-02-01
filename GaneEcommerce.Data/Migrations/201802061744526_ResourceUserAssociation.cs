namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceUserAssociation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "AuthUserId", c => c.Int());
            CreateIndex("dbo.Resources", "AuthUserId");
            AddForeignKey("dbo.Resources", "AuthUserId", "dbo.AuthUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Resources", "AuthUserId", "dbo.AuthUsers");
            DropIndex("dbo.Resources", new[] { "AuthUserId" });
            DropColumn("dbo.Resources", "AuthUserId");
        }
    }
}
