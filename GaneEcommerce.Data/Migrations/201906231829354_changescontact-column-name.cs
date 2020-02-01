namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changescontactcolumnname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountContacts", "ConTypeRemittance", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccountContacts", "ConTypeStatment", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccountContacts", "ConTypeInvoices", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccountContacts", "ConTypePurchasing", c => c.Boolean(nullable: false));
            DropColumn("dbo.AccountContacts", "ConTypeExecutive");
            DropColumn("dbo.AccountContacts", "ConTypeAdmin");
            DropColumn("dbo.AccountContacts", "ConTypeBilling");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountContacts", "ConTypeBilling", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccountContacts", "ConTypeAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccountContacts", "ConTypeExecutive", c => c.Boolean(nullable: false));
            DropColumn("dbo.AccountContacts", "ConTypePurchasing");
            DropColumn("dbo.AccountContacts", "ConTypeInvoices");
            DropColumn("dbo.AccountContacts", "ConTypeStatment");
            DropColumn("dbo.AccountContacts", "ConTypeRemittance");
        }
    }
}
