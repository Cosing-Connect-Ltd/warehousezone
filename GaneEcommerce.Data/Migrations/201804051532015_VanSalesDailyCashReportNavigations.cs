namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VanSalesDailyCashReportNavigations : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.VanSalesDailyCashes", "TerminalId");
            CreateIndex("dbo.VanSalesDailyCashes", "MobileLocationId");
            CreateIndex("dbo.VanSalesDailyCashes", "SalesManUserId");
            AddForeignKey("dbo.VanSalesDailyCashes", "SalesManUserId", "dbo.AuthUsers", "UserId");
            AddForeignKey("dbo.VanSalesDailyCashes", "MobileLocationId", "dbo.TenantLocations", "WarehouseId");
            AddForeignKey("dbo.VanSalesDailyCashes", "TerminalId", "dbo.Terminals", "TerminalId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VanSalesDailyCashes", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.VanSalesDailyCashes", "MobileLocationId", "dbo.TenantLocations");
            DropForeignKey("dbo.VanSalesDailyCashes", "SalesManUserId", "dbo.AuthUsers");
            DropIndex("dbo.VanSalesDailyCashes", new[] { "SalesManUserId" });
            DropIndex("dbo.VanSalesDailyCashes", new[] { "MobileLocationId" });
            DropIndex("dbo.VanSalesDailyCashes", new[] { "TerminalId" });
        }
    }
}
