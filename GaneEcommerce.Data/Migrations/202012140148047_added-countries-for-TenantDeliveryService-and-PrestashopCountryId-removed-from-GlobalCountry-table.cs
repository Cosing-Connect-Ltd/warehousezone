namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcountriesforTenantDeliveryServiceandPrestashopCountryIdremovedfromGlobalCountrytable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TenantDeliveryServiceCountryMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantDeliveryServiceId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryId)
                .ForeignKey("dbo.TenantDeliveryServices", t => t.TenantDeliveryServiceId)
                .Index(t => t.TenantDeliveryServiceId)
                .Index(t => t.CountryId);
            
            AddColumn("dbo.GlobalCountry", "AdditionalCountryCodes", c => c.String());
            AddColumn("dbo.TenantDeliveryServices", "SLAPriorityId", c => c.Int());
            CreateIndex("dbo.TenantDeliveryServices", "SLAPriorityId");
            AddForeignKey("dbo.TenantDeliveryServices", "SLAPriorityId", "dbo.SLAPriorits", "SLAPriorityId");
            DropColumn("dbo.GlobalCountry", "PrestaShopCountryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GlobalCountry", "PrestaShopCountryId", c => c.Int());
            DropForeignKey("dbo.TenantDeliveryServiceCountryMaps", "TenantDeliveryServiceId", "dbo.TenantDeliveryServices");
            DropForeignKey("dbo.TenantDeliveryServices", "SLAPriorityId", "dbo.SLAPriorits");
            DropForeignKey("dbo.TenantDeliveryServiceCountryMaps", "CountryId", "dbo.GlobalCountry");
            DropIndex("dbo.TenantDeliveryServices", new[] { "SLAPriorityId" });
            DropIndex("dbo.TenantDeliveryServiceCountryMaps", new[] { "CountryId" });
            DropIndex("dbo.TenantDeliveryServiceCountryMaps", new[] { "TenantDeliveryServiceId" });
            DropColumn("dbo.TenantDeliveryServices", "SLAPriorityId");
            DropColumn("dbo.GlobalCountry", "AdditionalCountryCodes");
            DropTable("dbo.TenantDeliveryServiceCountryMaps");
        }
    }
}
