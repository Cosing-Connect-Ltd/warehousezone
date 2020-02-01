namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class credittermsinaccounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "CreditTerms", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "CreditTerms");
        }
    }
}
