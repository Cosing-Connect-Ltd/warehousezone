namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderReceiveCountAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderReceiveCounts",
                c => new
                    {
                        ReceiveCountId = c.Guid(nullable: false),
                        OrderID = c.Int(nullable: false),
                        ReferenceNo = c.String(nullable: false),
                        Notes = c.String(),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ReceiveCountId)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .ForeignKey("dbo.AuthUsers", t => t.CreatedBy)
                .Index(t => t.OrderID)
                .Index(t => t.WarehouseId)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.OrderReceiveCountDetails",
                c => new
                    {
                        ReceiveCountDetailId = c.Guid(nullable: false),
                        ReceiveCountId = c.Guid(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Counted = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Demaged = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDetailID = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        TenentId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ReceiveCountDetailId)
                .ForeignKey("dbo.OrderDetails", t => t.OrderDetailID)
                .ForeignKey("dbo.OrderReceiveCounts", t => t.ReceiveCountId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ReceiveCountId)
                .Index(t => t.ProductId)
                .Index(t => t.OrderDetailID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderReceiveCounts", "CreatedBy", "dbo.AuthUsers");
            DropForeignKey("dbo.OrderReceiveCounts", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.OrderReceiveCountDetails", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.OrderReceiveCountDetails", "ReceiveCountId", "dbo.OrderReceiveCounts");
            DropForeignKey("dbo.OrderReceiveCountDetails", "OrderDetailID", "dbo.OrderDetails");
            DropForeignKey("dbo.OrderReceiveCounts", "OrderID", "dbo.Orders");
            DropIndex("dbo.OrderReceiveCountDetails", new[] { "OrderDetailID" });
            DropIndex("dbo.OrderReceiveCountDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderReceiveCountDetails", new[] { "ReceiveCountId" });
            DropIndex("dbo.OrderReceiveCounts", new[] { "CreatedBy" });
            DropIndex("dbo.OrderReceiveCounts", new[] { "WarehouseId" });
            DropIndex("dbo.OrderReceiveCounts", new[] { "OrderID" });
            DropTable("dbo.OrderReceiveCountDetails");
            DropTable("dbo.OrderReceiveCounts");
        }
    }
}
