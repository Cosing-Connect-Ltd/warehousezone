namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorchangesinAccountAddressAccountContactsTenantLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountAddresses", "Fax", c => c.String());
            AddColumn("dbo.TenantLocations", "AddressLine4", c => c.String(maxLength: 200));
            AddColumn("dbo.TenantLocations", "City", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountContacts", "TenantContactPin", c => c.Short());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountContacts", "TenantContactPin", c => c.Short(nullable: false));
            DropColumn("dbo.TenantLocations", "City");
            DropColumn("dbo.TenantLocations", "AddressLine4");
            DropColumn("dbo.AccountAddresses", "Fax");
        }
    }
}
