namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addinginvoicedateinorderprocess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcesses", "InvoiceDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcesses", "InvoiceDate");
        }
    }
}
