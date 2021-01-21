namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccountRewardPoints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountRewardPoints",
                c => new
                    {
                        AccountRewardPointID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        AccountID = c.Int(),
                        UserID = c.Int(nullable: false),
                        PointsEarned = c.Int(nullable: false),
                        OrderTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDateTime = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AccountRewardPointID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountRewardPoints");
        }
    }
}
