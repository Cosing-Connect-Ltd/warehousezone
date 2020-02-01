namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignkeysaddedforGlobalCurrencyandInvoiceMasterId : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AccountTransaction", "InvoiceMasterId");
            CreateIndex("dbo.InvoiceMaster", "CurrencyId");
            AddForeignKey("dbo.InvoiceMaster", "CurrencyId", "dbo.GlobalCurrency", "CurrencyID");
            AddForeignKey("dbo.AccountTransaction", "InvoiceMasterId", "dbo.InvoiceMaster", "InvoiceMasterId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountTransaction", "InvoiceMasterId", "dbo.InvoiceMaster");
            DropForeignKey("dbo.InvoiceMaster", "CurrencyId", "dbo.GlobalCurrency");
            DropIndex("dbo.InvoiceMaster", new[] { "CurrencyId" });
            DropIndex("dbo.AccountTransaction", new[] { "InvoiceMasterId" });
        }
    }
}
