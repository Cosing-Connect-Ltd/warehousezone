namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingtenantregistrationnoinTenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tenants", "TenantRegNo", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tenants", "TenantRegNo");
        }
    }
}
