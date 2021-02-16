namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRewardPointTriggers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RewardPointTriggers",
                c => new
                    {
                        RewardPointTriggerId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        TriggerType = c.Int(nullable: false),
                        ShoppingVoucherId = c.Int(nullable: false),
                        LoyaltyPointToTrigger = c.Int(),
                        MinimumOrderValueToTrigger = c.Decimal(precision: 18, scale: 2),
                        FoodOrderTypeToTrigger = c.Int(),
                        MaximumAllowed = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.RewardPointTriggerId)
                .ForeignKey("dbo.ShoppingVouchers", t => t.ShoppingVoucherId)
                .Index(t => t.ShoppingVoucherId);
            
            AlterColumn("dbo.ShoppingVouchers", "VoucherTitle", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RewardPointTriggers", "ShoppingVoucherId", "dbo.ShoppingVouchers");
            DropIndex("dbo.RewardPointTriggers", new[] { "ShoppingVoucherId" });
            AlterColumn("dbo.ShoppingVouchers", "VoucherTitle", c => c.String());
            DropTable("dbo.RewardPointTriggers");
        }
    }
}
