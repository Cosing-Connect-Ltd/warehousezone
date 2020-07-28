namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userverificationpropertiesadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "UserMobileNumber", c => c.String());
            AddColumn("dbo.AuthUsers", "WebUser", c => c.Boolean());
            AddColumn("dbo.AuthUsers", "VerificationRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.AuthUsers", "EmailVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.AuthUsers", "MobileNumberVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "MobileNumberVerified");
            DropColumn("dbo.AuthUsers", "EmailVerified");
            DropColumn("dbo.AuthUsers", "VerificationRequired");
            DropColumn("dbo.AuthUsers", "WebUser");
            DropColumn("dbo.AuthUsers", "UserMobileNumber");
        }
    }
}
