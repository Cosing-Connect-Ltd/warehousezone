namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoicenumberaddedinorderprocess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcesses", "InvoiceNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "InvoiceNo");
        }
    }
}
