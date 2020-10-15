namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedArchivableItemsAgeDaystoTenantConfigstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "ArchivableItemsAgeDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "ArchivableItemsAgeDays");
        }
    }
}
