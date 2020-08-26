namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class added_our_brands_relates_flags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantWebsites", "HomeOurBrandsText", c => c.String());
            AddColumn("dbo.ProductManufacturers", "ShowInOurBrands", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductManufacturers", "ShowInOurBrands");
            DropColumn("dbo.TenantWebsites", "HomeOurBrandsText");
        }
    }
}
