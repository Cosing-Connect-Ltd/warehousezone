namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingcurrencyidinmasterinvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceMaster", "CurrencyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceMaster", "CurrencyId");
        }
    }
}
