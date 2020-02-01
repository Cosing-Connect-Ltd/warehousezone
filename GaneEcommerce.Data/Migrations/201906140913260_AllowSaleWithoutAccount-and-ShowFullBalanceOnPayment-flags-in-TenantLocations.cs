namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowSaleWithoutAccountandShowFullBalanceOnPaymentflagsinTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "ShowFullBalanceOnPayment", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenantLocations", "AllowSaleWithoutAccount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "AllowSaleWithoutAccount");
            DropColumn("dbo.TenantLocations", "ShowFullBalanceOnPayment");
        }
    }
}
