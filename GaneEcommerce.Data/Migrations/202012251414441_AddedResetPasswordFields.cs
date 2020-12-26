namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedResetPasswordFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "ResetPasswordCode", c => c.String());
            AddColumn("dbo.AuthUsers", "ResetPasswordCodeExpiry", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "ResetPasswordCodeExpiry");
            DropColumn("dbo.AuthUsers", "ResetPasswordCode");
        }
    }
}
