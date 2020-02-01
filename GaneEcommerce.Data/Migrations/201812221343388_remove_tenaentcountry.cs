namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_tenaentcountry : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tenants", "TenantCountry");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tenants", "TenantCountry", c => c.String());
        }
    }
}
