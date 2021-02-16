namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoyaltyReferralTrackingInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "ReferralConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "ReferralConfirmed");
        }
    }
}
