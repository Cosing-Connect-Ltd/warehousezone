namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addressline4droppedfromPTenantsPLandlordsandtenantlocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PLandlords", "AddressTown", c => c.String());
            AddColumn("dbo.PTenants", "AddressTown", c => c.String());
            DropColumn("dbo.TenantLocations", "AddressLine4");
            DropColumn("dbo.PLandlords", "AddressLine4");
            DropColumn("dbo.PTenants", "AddressLine4");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PTenants", "AddressLine4", c => c.String());
            AddColumn("dbo.PLandlords", "AddressLine4", c => c.String());
            AddColumn("dbo.TenantLocations", "AddressLine4", c => c.String(maxLength: 200));
            DropColumn("dbo.PTenants", "AddressTown");
            DropColumn("dbo.PLandlords", "AddressTown");
        }
    }
}
