namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VanSalesDailyCashReportEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VanSalesDailyCashes",
                c => new
                    {
                        VanSalesDailyCashId = c.Int(nullable: false, identity: true),
                        SaleDate = c.DateTime(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        MobileLocationId = c.Int(nullable: false),
                        SalesManUserId = c.Int(nullable: false),
                        FiveHundred = c.Int(nullable: false),
                        TwoHundred = c.Int(nullable: false),
                        OneHundred = c.Int(nullable: false),
                        Fifty = c.Int(nullable: false),
                        Twenty = c.Int(nullable: false),
                        Ten = c.Int(nullable: false),
                        Five = c.Int(nullable: false),
                        Two = c.Int(nullable: false),
                        One = c.Int(nullable: false),
                        PointFifty = c.Int(nullable: false),
                        PointTwentyFive = c.Int(nullable: false),
                        PointTwenty = c.Int(nullable: false),
                        PointTen = c.Int(nullable: false),
                        PointFive = c.Int(nullable: false),
                        PointTwo = c.Int(nullable: false),
                        PointOne = c.Int(nullable: false),
                        TotalSale = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPaidCash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPaidCheques = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPaidCards = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCashSubmitted = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChequesCount = c.Int(nullable: false),
                        Notes = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.VanSalesDailyCashId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VanSalesDailyCashes");
        }
    }
}
