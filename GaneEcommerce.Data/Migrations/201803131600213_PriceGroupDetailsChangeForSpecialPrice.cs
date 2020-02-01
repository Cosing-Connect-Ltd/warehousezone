namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceGroupDetailsChangeForSpecialPrice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductSpecialPrices", "AccountID", "dbo.Account");
            DropForeignKey("dbo.ProductSpecialPrices", "ProductID", "dbo.ProductMaster");
            DropIndex("dbo.ProductSpecialPrices", new[] { "ProductID" });
            DropIndex("dbo.ProductSpecialPrices", new[] { "AccountID" });
            CreateTable(
                "dbo.PriceGroupDetails",
                c => new
                    {
                        PriceGroupDetailID = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductID = c.Int(nullable: false),
                        PriceGroupID = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.PriceGroupDetailID)
                .ForeignKey("dbo.TenantPriceGroups", t => t.PriceGroupID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductID)
                .Index(t => t.ProductID)
                .Index(t => t.PriceGroupID);
            
            DropTable("dbo.ProductSpecialPrices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductSpecialPrices",
                c => new
                    {
                        ProductSpecialPriceID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        AccountID = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductSpecialPriceID);
            
            DropForeignKey("dbo.PriceGroupDetails", "ProductID", "dbo.ProductMaster");
            DropForeignKey("dbo.PriceGroupDetails", "PriceGroupID", "dbo.TenantPriceGroups");
            DropIndex("dbo.PriceGroupDetails", new[] { "PriceGroupID" });
            DropIndex("dbo.PriceGroupDetails", new[] { "ProductID" });
            DropTable("dbo.PriceGroupDetails");
            CreateIndex("dbo.ProductSpecialPrices", "AccountID");
            CreateIndex("dbo.ProductSpecialPrices", "ProductID");
            AddForeignKey("dbo.ProductSpecialPrices", "ProductID", "dbo.ProductMaster", "ProductId");
            AddForeignKey("dbo.ProductSpecialPrices", "AccountID", "dbo.Account", "AccountID");
        }
    }
}
