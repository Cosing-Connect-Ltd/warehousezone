namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountTransactions_AccountId_nullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AccountTransaction", new[] { "AccountId" });
            AlterColumn("dbo.AccountTransaction", "AccountId", c => c.Int());
            CreateIndex("dbo.AccountTransaction", "AccountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AccountTransaction", new[] { "AccountId" });
            AlterColumn("dbo.AccountTransaction", "AccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.AccountTransaction", "AccountId");
        }
    }
}
