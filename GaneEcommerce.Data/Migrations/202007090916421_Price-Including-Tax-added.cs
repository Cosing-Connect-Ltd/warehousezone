namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceIncludingTaxadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "ShowPricesIncludingTax", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantWebsites", "ShowPricesIncludingTax");
        }
    }
}
