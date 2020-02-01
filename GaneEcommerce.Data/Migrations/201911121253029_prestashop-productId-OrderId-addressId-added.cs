namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prestashopproductIdOrderIdaddressIdadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountAddresses", "PrestaShopAddressId", c => c.Int());
            AddColumn("dbo.ProductMaster", "PrestaShopProductId", c => c.Int());
            AddColumn("dbo.Orders", "PrestaShopOrderId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PrestaShopOrderId");
            DropColumn("dbo.ProductMaster", "PrestaShopProductId");
            DropColumn("dbo.AccountAddresses", "PrestaShopAddressId");
        }
    }
}
