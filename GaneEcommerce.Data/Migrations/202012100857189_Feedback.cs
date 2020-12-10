namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feedback : DbMigration
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
                        FoodRate = c.String(),
                        AppRate = c.String(),
                        FeedbackMessge = c.String(),
                        TenantId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .Index(t => t.AccountID)
                .Index(t => t.OrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Feedbacks", "AccountID", "dbo.Account");
            DropIndex("dbo.Feedbacks", new[] { "OrderID" });
            DropIndex("dbo.Feedbacks", new[] { "AccountID" });
            DropTable("dbo.Feedbacks");
        }
    }
}
