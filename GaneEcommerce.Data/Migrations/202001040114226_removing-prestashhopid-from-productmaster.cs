namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingprestashhopidfromproductmaster : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductMaster", "PrestaShopProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductMaster", "PrestaShopProductId", c => c.Int());
        }
    }
}
