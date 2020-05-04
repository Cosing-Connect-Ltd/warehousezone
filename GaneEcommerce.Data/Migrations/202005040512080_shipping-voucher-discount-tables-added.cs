namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shippingvoucherdiscounttablesadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteDiscountProductsMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        DiscountId = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.WebsiteDiscountCodes", t => t.DiscountId)
                .Index(t => t.ProductId)
                .Index(t => t.DiscountId);
            
            CreateTable(
                "dbo.WebsiteDiscountCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        Title = c.String(),
                        Code = c.String(),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        MinimumBasketValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FreeShippig = c.Boolean(nullable: false),
                        SingleUse = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DiscountType = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID);
            
            CreateTable(
                "dbo.WebsiteShippingRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteID = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        Courier = c.String(),
                        Region = c.String(),
                        PostalArea = c.String(),
                        WeightinGrams = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.WebsiteVouchers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SiteID = c.Int(nullable: false),
                        Code = c.String(),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shared = c.Boolean(nullable: false),
                        UserId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .ForeignKey("dbo.TenantWebsites", t => t.SiteID)
                .Index(t => t.SiteID)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.WebsiteContentPages", "Contant", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebsiteVouchers", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteVouchers", "UserId", "dbo.AuthUsers");
            DropForeignKey("dbo.WebsiteShippingRules", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteShippingRules", "CountryId", "dbo.GlobalCountry");
            DropForeignKey("dbo.WebsiteDiscountProductsMaps", "DiscountId", "dbo.WebsiteDiscountCodes");
            DropForeignKey("dbo.WebsiteDiscountCodes", "SiteID", "dbo.TenantWebsites");
            DropForeignKey("dbo.WebsiteDiscountProductsMaps", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.WebsiteVouchers", new[] { "UserId" });
            DropIndex("dbo.WebsiteVouchers", new[] { "SiteID" });
            DropIndex("dbo.WebsiteShippingRules", new[] { "CountryId" });
            DropIndex("dbo.WebsiteShippingRules", new[] { "SiteID" });
            DropIndex("dbo.WebsiteDiscountCodes", new[] { "SiteID" });
            DropIndex("dbo.WebsiteDiscountProductsMaps", new[] { "DiscountId" });
            DropIndex("dbo.WebsiteDiscountProductsMaps", new[] { "ProductId" });
            AlterColumn("dbo.WebsiteContentPages", "Contant", c => c.String());
            DropTable("dbo.WebsiteVouchers");
            DropTable("dbo.WebsiteShippingRules");
            DropTable("dbo.WebsiteDiscountCodes");
            DropTable("dbo.WebsiteDiscountProductsMaps");
        }
    }
}
