namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financialtransactionchequeproof : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountTransactionFile",
                c => new
                    {
                        AccountTransactionFileID = c.Int(nullable: false, identity: true),
                        AccountTransactionID = c.Int(nullable: false),
                        Title = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                        FileContent = c.Binary(),
                        ChequeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountID = c.Int(),
                        OrderID = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AccountTransactionFileID)
                .ForeignKey("dbo.AccountTransaction", t => t.AccountTransactionID)
                .Index(t => t.AccountTransactionID);
            
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountTransactionFile", "AccountTransactionID", "dbo.AccountTransaction");
            DropIndex("dbo.AccountTransactionFile", new[] { "AccountTransactionID" });
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String(nullable: false));
            DropTable("dbo.AccountTransactionFile");
        }
    }
}
