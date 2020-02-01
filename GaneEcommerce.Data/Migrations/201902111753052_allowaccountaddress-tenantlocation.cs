namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allowaccountaddresstenantlocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "AllowShipToAccountAddress", c => c.Boolean(nullable: false));
          
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "AllowShipToAccountAddress");
        }
    }
}
