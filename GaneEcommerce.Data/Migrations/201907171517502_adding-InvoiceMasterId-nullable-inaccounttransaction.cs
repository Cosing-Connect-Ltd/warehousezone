namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingInvoiceMasterIdnullableinaccounttransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountTransaction", "InvoiceMasterId", c => c.Int());
           
        }
        
        public override void Down()
        {
           
            DropColumn("dbo.AccountTransaction", "InvoiceMasterId");
        }
    }
}
