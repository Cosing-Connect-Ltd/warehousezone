namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class country_currencyRelation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Tenants", "CurrencyID");
            CreateIndex("dbo.Tenants", "CountryID");
            AddForeignKey("dbo.Tenants", "CountryID", "dbo.GlobalCountry", "CountryID");
            AddForeignKey("dbo.Tenants", "CurrencyID", "dbo.GlobalCurrency", "CurrencyID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tenants", "CurrencyID", "dbo.GlobalCurrency");
            DropForeignKey("dbo.Tenants", "CountryID", "dbo.GlobalCountry");
            DropIndex("dbo.Tenants", new[] { "CountryID" });
            DropIndex("dbo.Tenants", new[] { "CurrencyID" });
        }
    }
}
