namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullablefalsefields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "AccountCode", c => c.String(nullable: false));
            AlterColumn("dbo.ProductAccountCodes", "ProdAccCode", c => c.String(nullable: false));
            AlterColumn("dbo.ProductMaster", "SKUCode", c => c.String(nullable: false));
            AlterColumn("dbo.ProductMaster", "BarCode", c => c.String(nullable: false));
            AlterColumn("dbo.LocationGroups", "Locdescription", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.LocationTypes", "LocTypeName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Terminals", "TermainlSerial", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AuthUsers", "UserName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String(nullable: false));
            AlterColumn("dbo.ProductGroups", "ProductGroup", c => c.String(nullable: false));
            AlterColumn("dbo.TenantEmailTemplates", "Body", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TenantEmailTemplates", "Body", c => c.String());
            AlterColumn("dbo.ProductGroups", "ProductGroup", c => c.String());
            AlterColumn("dbo.Orders", "OrderNumber", c => c.String());
            AlterColumn("dbo.AuthUsers", "UserName", c => c.String());
            AlterColumn("dbo.Terminals", "TermainlSerial", c => c.String());
            AlterColumn("dbo.LocationTypes", "LocTypeName", c => c.String());
            AlterColumn("dbo.LocationGroups", "Locdescription", c => c.String());
            AlterColumn("dbo.ProductMaster", "BarCode", c => c.String());
            AlterColumn("dbo.ProductMaster", "SKUCode", c => c.String());
            AlterColumn("dbo.ProductAccountCodes", "ProdAccCode", c => c.String());
            AlterColumn("dbo.Account", "AccountCode", c => c.String());
        }
    }
}
