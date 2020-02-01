namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailAutoCheckedOnEditaddedintenantconfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "EmailAutoCheckedOnEdit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "EmailAutoCheckedOnEdit");
        }
    }
}
