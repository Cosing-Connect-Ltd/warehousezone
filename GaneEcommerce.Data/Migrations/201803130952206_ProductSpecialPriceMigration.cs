namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductSpecialPriceMigration : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductSpecialPrices");
        }
    }
}
