namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ProductAvailabilityNotifyQueue_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAvailabilityNotifyQueues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductAvailabilityNotifyQueues");
        }
    }
}
