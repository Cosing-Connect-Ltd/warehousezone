namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccountRewardLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountRewardPoints", "RewardProductId", c => c.Int());
            CreateIndex("dbo.AccountRewardPoints", "RewardProductId");
            AddForeignKey("dbo.AccountRewardPoints", "RewardProductId", "dbo.ProductMaster", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountRewardPoints", "RewardProductId", "dbo.ProductMaster");
            DropIndex("dbo.AccountRewardPoints", new[] { "RewardProductId" });
            DropColumn("dbo.AccountRewardPoints", "RewardProductId");
        }
    }
}
