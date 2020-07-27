namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModilesTableDeletedplusdeliverycollectionpricelistsaddedintenantlocations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuthActivities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.TenantModules", "ModuleId", "dbo.Modules");
            DropIndex("dbo.AuthActivities", new[] { "ModuleId" });
            DropIndex("dbo.TenantModules", new[] { "ModuleId" });
            AddColumn("dbo.TenantLocations", "EatInCharges", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TenantLocations", "DeliveryPriceGroupId", c => c.Int());
            AddColumn("dbo.TenantLocations", "CollectionPriceGroupId", c => c.Int());
            AddColumn("dbo.TenantLocations", "EatInPriceGroupId", c => c.Int());
            CreateIndex("dbo.TenantLocations", "DeliveryPriceGroupId");
            CreateIndex("dbo.TenantLocations", "CollectionPriceGroupId");
            CreateIndex("dbo.TenantLocations", "EatInPriceGroupId");
            AddForeignKey("dbo.TenantLocations", "CollectionPriceGroupId", "dbo.TenantPriceGroups", "PriceGroupID");
            AddForeignKey("dbo.TenantLocations", "DeliveryPriceGroupId", "dbo.TenantPriceGroups", "PriceGroupID");
            AddForeignKey("dbo.TenantLocations", "EatInPriceGroupId", "dbo.TenantPriceGroups", "PriceGroupID");
            DropTable("dbo.Modules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TenantLocations", "EatInPriceGroupId", "dbo.TenantPriceGroups");
            DropForeignKey("dbo.TenantLocations", "DeliveryPriceGroupId", "dbo.TenantPriceGroups");
            DropForeignKey("dbo.TenantLocations", "CollectionPriceGroupId", "dbo.TenantPriceGroups");
            DropIndex("dbo.TenantLocations", new[] { "EatInPriceGroupId" });
            DropIndex("dbo.TenantLocations", new[] { "CollectionPriceGroupId" });
            DropIndex("dbo.TenantLocations", new[] { "DeliveryPriceGroupId" });
            DropColumn("dbo.TenantLocations", "EatInPriceGroupId");
            DropColumn("dbo.TenantLocations", "CollectionPriceGroupId");
            DropColumn("dbo.TenantLocations", "DeliveryPriceGroupId");
            DropColumn("dbo.TenantLocations", "EatInCharges");
            CreateIndex("dbo.TenantModules", "ModuleId");
            CreateIndex("dbo.AuthActivities", "ModuleId");
            AddForeignKey("dbo.TenantModules", "ModuleId", "dbo.Modules", "Id");
            AddForeignKey("dbo.AuthActivities", "ModuleId", "dbo.Modules", "Id");
        }
    }
}
