namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbacksAndTenantLocationConfigs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
                        OrderID = c.Int(nullable: false),
                        ServiceRate = c.String(),
                        CustomerName = c.String(),
                        FoodRate = c.String(),
                        AppRate = c.String(),
                        FeedbackMessage = c.String(),
                        TenantID = c.Int(nullable: false),
                        TenantLocationID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .Index(t => t.AccountID)
                .Index(t => t.OrderID);
            
            AddColumn("dbo.TenantLocations", "DeliveryMinimumOrderValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.TenantLocations", "DeliveryMinimumOrderValueForFree", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Feedbacks", "AccountID", "dbo.Account");
            DropIndex("dbo.Feedbacks", new[] { "OrderID" });
            DropIndex("dbo.Feedbacks", new[] { "AccountID" });
            DropColumn("dbo.TenantLocations", "DeliveryMinimumOrderValueForFree");
            DropColumn("dbo.TenantLocations", "DeliveryMinimumOrderValue");
            DropTable("dbo.Feedbacks");
        }
    }
}
