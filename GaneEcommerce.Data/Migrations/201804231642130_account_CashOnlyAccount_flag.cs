namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class account_CashOnlyAccount_flag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "CashOnlyAccount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "CashOnlyAccount");
        }
    }
}
