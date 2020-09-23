namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_SessionId_to_AccountAddresses_table : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AccountAddresses", new[] { "AccountID" });
            AddColumn("dbo.AccountAddresses", "SessionId", c => c.String());
            AlterColumn("dbo.AccountAddresses", "AccountID", c => c.Int());
            CreateIndex("dbo.AccountAddresses", "AccountID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AccountAddresses", new[] { "AccountID" });
            AlterColumn("dbo.AccountAddresses", "AccountID", c => c.Int(nullable: false));
            DropColumn("dbo.AccountAddresses", "SessionId");
            CreateIndex("dbo.AccountAddresses", "AccountID");
        }
    }
}
