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
        }
        
        public override void Down()
        {
            DropColumn("dbo.RewardPointTriggers", "IsDeleted");
            DropColumn("dbo.RewardPointTriggers", "WarehouseId");
            DropColumn("dbo.RewardPointTriggers", "TenantId");
        }
    }
}
