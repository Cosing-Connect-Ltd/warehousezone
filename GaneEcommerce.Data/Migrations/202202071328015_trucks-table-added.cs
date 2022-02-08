namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class truckstableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trucks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TruckName = c.String(),
                        SortOrder = c.Short(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.AuthUsers", "WarehouseId");
            DropColumn("dbo.PalletTrackings", "OrderNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PalletTrackings", "OrderNumber", c => c.String());
            AddColumn("dbo.AuthUsers", "WarehouseId", c => c.Int());
            DropTable("dbo.Trucks");
        }
    }
}
