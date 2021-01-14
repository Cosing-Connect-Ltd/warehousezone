namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShoppingVouchersForMultiUsage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingVouchers", "MaximumAllowedUse", c => c.Int());
            AddColumn("dbo.ShoppingVouchers", "VoucherUsedCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingVouchers", "VoucherUsedCount");
            DropColumn("dbo.ShoppingVouchers", "MaximumAllowedUse");
        }
    }
}
