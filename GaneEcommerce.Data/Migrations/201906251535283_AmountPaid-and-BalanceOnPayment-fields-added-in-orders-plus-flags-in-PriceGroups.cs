namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AmountPaidandBalanceOnPaymentfieldsaddedinordersplusflagsinPriceGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AmountPaidByAccount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "AccountBalanceOnPayment", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "AccountPaymentModeId", c => c.Int());
            AddColumn("dbo.Orders", "EndOfDayGenerated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "VanSalesDailyCashId", c => c.Int());
            AddColumn("dbo.TenantPriceGroups", "ApplyDiscountOnTotal", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantPriceGroups", "ApplyDiscountOnSpecialPrice", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Orders", "VanSalesDailyCashId");
            AddForeignKey("dbo.Orders", "VanSalesDailyCashId", "dbo.VanSalesDailyCashes", "VanSalesDailyCashId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "VanSalesDailyCashId", "dbo.VanSalesDailyCashes");
            DropIndex("dbo.Orders", new[] { "VanSalesDailyCashId" });
            DropColumn("dbo.TenantPriceGroups", "ApplyDiscountOnSpecialPrice");
            DropColumn("dbo.TenantPriceGroups", "ApplyDiscountOnTotal");
            DropColumn("dbo.Orders", "VanSalesDailyCashId");
            DropColumn("dbo.Orders", "EndOfDayGenerated");
            DropColumn("dbo.Orders", "AccountPaymentModeId");
            DropColumn("dbo.Orders", "AccountBalanceOnPayment");
            DropColumn("dbo.Orders", "AmountPaidByAccount");
        }
    }
}
