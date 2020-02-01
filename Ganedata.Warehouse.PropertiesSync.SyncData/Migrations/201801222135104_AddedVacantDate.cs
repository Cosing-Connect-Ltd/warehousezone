namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVacantDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PTenant", "TenancyVacateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PTenant", "TenancyVacateDate");
        }
    }
}
