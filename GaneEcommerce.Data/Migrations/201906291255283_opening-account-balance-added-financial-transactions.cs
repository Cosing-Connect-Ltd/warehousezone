namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class openingaccountbalanceaddedfinancialtransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountTransaction", "OpeningBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "AccountBalanceBeforePayment", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "AccountBalanceBeforePayment");
            DropColumn("dbo.AccountTransaction", "OpeningBalance");
        }
    }
}
