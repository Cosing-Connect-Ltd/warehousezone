namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Statustypestablesdropped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "AccountPaymentModeId", "dbo.AccountPaymentModes");
            DropForeignKey("dbo.OrderProcesses", "OrderProcessStatusId", "dbo.OrderProcessStatus");
            DropForeignKey("dbo.Account", "AccountStatusID", "dbo.GlobalAccountStatus");
            DropForeignKey("dbo.AccountStatusAudit", "NewStatusId", "dbo.GlobalAccountStatus");
            DropForeignKey("dbo.AccountTransaction", "AccountPaymentModeId", "dbo.AccountPaymentModes");
            DropForeignKey("dbo.AccountTransaction", "AccountTransactionTypeId", "dbo.AccountTransactionTypes");
            DropForeignKey("dbo.MarketJobAllocation", "MarketJobStatusId", "dbo.MarketJobStatus");
            DropIndex("dbo.Account", new[] { "AccountStatusID" });
            DropIndex("dbo.Orders", new[] { "AccountPaymentModeId" });
            DropIndex("dbo.OrderProcesses", new[] { "OrderProcessStatusId" });
            DropIndex("dbo.AccountStatusAudit", new[] { "NewStatusId" });
            DropIndex("dbo.AccountTransaction", new[] { "AccountPaymentModeId" });
            DropIndex("dbo.AccountTransaction", new[] { "AccountTransactionTypeId" });
            DropIndex("dbo.MarketJobAllocation", new[] { "MarketJobStatusId" });
            DropTable("dbo.AccountPaymentModes");
            DropTable("dbo.OrderProcessStatus");
            DropTable("dbo.GlobalAccountStatus");
            DropTable("dbo.AccountTransactionTypes");
            DropTable("dbo.MarketJobStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MarketJobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccountTransactionTypes",
                c => new
                    {
                        AccountTransactionTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AccountTransactionTypeId);
            
            CreateTable(
                "dbo.GlobalAccountStatus",
                c => new
                    {
                        AccountStatusID = c.Int(nullable: false, identity: true),
                        AccountStatus = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.AccountStatusID);
            
            CreateTable(
                "dbo.OrderProcessStatus",
                c => new
                    {
                        OrderProcessStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.OrderProcessStatusId);
            
            CreateTable(
                "dbo.AccountPaymentModes",
                c => new
                    {
                        AccountPaymentModeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AccountPaymentModeId);
            
            CreateIndex("dbo.MarketJobAllocation", "MarketJobStatusId");
            CreateIndex("dbo.AccountTransaction", "AccountTransactionTypeId");
            CreateIndex("dbo.AccountTransaction", "AccountPaymentModeId");
            CreateIndex("dbo.AccountStatusAudit", "NewStatusId");
            CreateIndex("dbo.OrderProcesses", "OrderProcessStatusId");
            CreateIndex("dbo.Orders", "AccountPaymentModeId");
            CreateIndex("dbo.Account", "AccountStatusID");
            AddForeignKey("dbo.MarketJobAllocation", "MarketJobStatusId", "dbo.MarketJobStatus", "Id");
            AddForeignKey("dbo.AccountTransaction", "AccountTransactionTypeId", "dbo.AccountTransactionTypes", "AccountTransactionTypeId");
            AddForeignKey("dbo.AccountTransaction", "AccountPaymentModeId", "dbo.AccountPaymentModes", "AccountPaymentModeId");
            AddForeignKey("dbo.AccountStatusAudit", "NewStatusId", "dbo.GlobalAccountStatus", "AccountStatusID");
            AddForeignKey("dbo.Account", "AccountStatusID", "dbo.GlobalAccountStatus", "AccountStatusID");
            AddForeignKey("dbo.OrderProcesses", "OrderProcessStatusId", "dbo.OrderProcessStatus", "OrderProcessStatusId");
            AddForeignKey("dbo.Orders", "AccountPaymentModeId", "dbo.AccountPaymentModes", "AccountPaymentModeId");
        }
    }
}
