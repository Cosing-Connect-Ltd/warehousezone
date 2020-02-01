namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingaccountidinauthusers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "AccountId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "AccountId");
        }
    }
}
