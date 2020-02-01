namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class holidaysstartDateintenantLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantLocations", "StartDateofHolidaysYear", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantLocations", "StartDateofHolidaysYear");
        }
    }
}
