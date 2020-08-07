namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loyaltyfieldsinaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "AccountLoyaltyCode", c => c.String());
            AddColumn("dbo.Account", "AccountLoyaltyPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "AccountLoyaltyPoints");
            DropColumn("dbo.Account", "AccountLoyaltyCode");
        }
    }
}
