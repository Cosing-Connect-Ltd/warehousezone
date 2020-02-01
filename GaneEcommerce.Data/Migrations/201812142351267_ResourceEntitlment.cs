namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceEntitlment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "HolidayEntitlement", c => c.Double());
            DropColumn("dbo.ResourceRequests", "HolidayEntitlement");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResourceRequests", "HolidayEntitlement", c => c.Double());
            DropColumn("dbo.Resources", "HolidayEntitlement");
        }
    }
}
