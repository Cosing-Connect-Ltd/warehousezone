namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullallowedinaccountaddresses : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AccountAddresses", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountAddresses", "Name", c => c.String(nullable: false));
        }
    }
}
