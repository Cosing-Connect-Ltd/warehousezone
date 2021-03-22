namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedRewardTriggers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RewardPointTriggers", "TenantId", c => c.Int());
            AddColumn("dbo.RewardPointTriggers", "WarehouseId", c => c.Int());
            AddColumn("dbo.RewardPointTriggers", "IsDeleted", c => c.Boolean());

            AddColumn("dbo.AccountRewardPoints", "RewardProductId", c => c.Int());
            CreateIndex("dbo.AccountRewardPoints", "RewardProductId");
            AddForeignKey("dbo.AccountRewardPoints", "RewardProductId", "dbo.ProductMaster", "ProductId");
        }
        
        public override void Down()
        {
            DropColumn("dbo.RewardPointTriggers", "IsDeleted");
            DropColumn("dbo.RewardPointTriggers", "WarehouseId");
            DropColumn("dbo.RewardPointTriggers", "TenantId");

            DropForeignKey("dbo.AccountRewardPoints", "RewardProductId", "dbo.ProductMaster");
            DropIndex("dbo.AccountRewardPoints", new[] { "RewardProductId" });
            DropColumn("dbo.AccountRewardPoints", "RewardProductId");
        }
    }
}
