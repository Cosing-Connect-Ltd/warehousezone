namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressLine4inAccountAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountAddresses", "AddressLine4", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountAddresses", "AddressLine4");
        }
    }
}
