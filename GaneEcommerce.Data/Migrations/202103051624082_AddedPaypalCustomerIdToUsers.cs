namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPaypalCustomerIdToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "PaypalCustomerId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "PaypalCustomerId");
        }
    }
}
