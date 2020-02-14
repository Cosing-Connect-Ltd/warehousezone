namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingaccountownerrelationwithauthuser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "AcceptedShelfLife", c => c.Int());
            CreateIndex("dbo.Account", "OwnerUserId");
            AddForeignKey("dbo.Account", "OwnerUserId", "dbo.AuthUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Account", "OwnerUserId", "dbo.AuthUsers");
            DropIndex("dbo.Account", new[] { "OwnerUserId" });
            AlterColumn("dbo.Account", "AcceptedShelfLife", c => c.Int(nullable: false));
        }
    }
}
