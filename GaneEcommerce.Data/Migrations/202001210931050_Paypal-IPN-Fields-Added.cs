namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaypalIPNFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountTransaction", "PaymentDate", c => c.DateTime());
            AddColumn("dbo.AccountTransaction", "Memo", c => c.String());
            AddColumn("dbo.AccountTransaction", "Status", c => c.String());
            AddColumn("dbo.AccountTransaction", "IPNData", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountTransaction", "IPNData");
            DropColumn("dbo.AccountTransaction", "Status");
            DropColumn("dbo.AccountTransaction", "Memo");
            DropColumn("dbo.AccountTransaction", "PaymentDate");
        }
    }
}
