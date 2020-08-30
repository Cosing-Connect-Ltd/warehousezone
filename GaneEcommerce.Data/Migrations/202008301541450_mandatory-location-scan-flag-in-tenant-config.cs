namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mandatorylocationscanflagintenantconfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "MandatoryLocationScan", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "MandatoryLocationScan");
        }
    }
}
