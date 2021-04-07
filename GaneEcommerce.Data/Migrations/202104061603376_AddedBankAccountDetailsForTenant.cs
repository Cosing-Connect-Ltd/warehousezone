namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBankAccountDetailsForTenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "BankAccountName", c => c.String());
            AddColumn("dbo.TenantConfigs", "BankAccountNumber", c => c.String());
            AddColumn("dbo.TenantConfigs", "BankAccountSortCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "BankAccountSortCode");
            DropColumn("dbo.TenantConfigs", "BankAccountNumber");
            DropColumn("dbo.TenantConfigs", "BankAccountName");
        }
    }
}
