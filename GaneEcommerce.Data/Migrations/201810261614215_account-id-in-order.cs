namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountidinorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AccountAddressId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "AccountAddressId");
        }
    }
}
