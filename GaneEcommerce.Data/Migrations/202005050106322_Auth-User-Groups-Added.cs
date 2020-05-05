namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthUserGroupsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthUserGroups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GroupAdministrators = c.String(),
                        AllowEnquiries = c.Boolean(nullable: false),
                        AllowBaskets = c.Boolean(nullable: false),
                        AllowCheckout = c.Boolean(nullable: false),
                        DefaultForWebRegistration = c.Boolean(nullable: false),
                        SuperUserAccess = c.Boolean(nullable: false),
                        MaxProductsPerOrder = c.Int(nullable: false),
                        ShippingMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductMarkup = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AllowPriceBreaks = c.Boolean(nullable: false),
                        BillingCompanyName = c.String(),
                        BillingAddressLine1 = c.String(),
                        BillingAddressLine2 = c.String(),
                        City = c.String(),
                        Region = c.String(),
                        PostCode = c.String(),
                        CountryId = c.Int(),
                        AccountNumber = c.String(),
                        AllowSagePay = c.Boolean(nullable: false),
                        AllowPayPal = c.Boolean(nullable: false),
                        AllowInvoice = c.Boolean(nullable: false),
                        AllowProformaInvoice = c.Boolean(nullable: false),
                        AllowFoC = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryId)
                .Index(t => t.CountryId);
            
            AddColumn("dbo.AuthUsers", "UserGroupId", c => c.Int());
            CreateIndex("dbo.AuthUsers", "UserGroupId");
            AddForeignKey("dbo.AuthUsers", "UserGroupId", "dbo.AuthUserGroups", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthUserGroups", "CountryId", "dbo.GlobalCountry");
            DropForeignKey("dbo.AuthUsers", "UserGroupId", "dbo.AuthUserGroups");
            DropIndex("dbo.AuthUserGroups", new[] { "CountryId" });
            DropIndex("dbo.AuthUsers", new[] { "UserGroupId" });
            DropColumn("dbo.AuthUsers", "UserGroupId");
            DropTable("dbo.AuthUserGroups");
        }
    }
}
