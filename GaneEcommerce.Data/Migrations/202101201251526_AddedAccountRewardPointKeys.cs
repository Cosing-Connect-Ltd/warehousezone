namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccountRewardPointKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AccountRewardPoints", "AccountID");
            AddForeignKey("dbo.AccountRewardPoints", "AccountID", "dbo.Account", "AccountID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountRewardPoints", "AccountID", "dbo.Account");
            DropIndex("dbo.AccountRewardPoints", new[] { "AccountID" });
        }
    }
}
