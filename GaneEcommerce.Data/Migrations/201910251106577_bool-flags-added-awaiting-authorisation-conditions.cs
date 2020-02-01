namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boolflagsaddedawaitingauthorisationconditions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "OrderValueLimit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TenantConfigs", "OrdersAuthroisingRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "OrdersAuthroisingRequired");
            DropColumn("dbo.AuthUsers", "OrderValueLimit");
        }
    }
}
