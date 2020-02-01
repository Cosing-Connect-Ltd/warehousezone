namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantPriceGroupIdchangedtononNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TenantPriceGroupDetails", new[] { "PriceGroupID" });
            AlterColumn("dbo.TenantPriceGroupDetails", "PriceGroupID", c => c.Int(nullable: false));
            CreateIndex("dbo.TenantPriceGroupDetails", "PriceGroupID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TenantPriceGroupDetails", new[] { "PriceGroupID" });
            AlterColumn("dbo.TenantPriceGroupDetails", "PriceGroupID", c => c.Int());
            CreateIndex("dbo.TenantPriceGroupDetails", "PriceGroupID");
        }
    }
}
