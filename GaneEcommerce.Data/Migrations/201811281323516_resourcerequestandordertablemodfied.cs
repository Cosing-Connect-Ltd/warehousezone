namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourcerequestandordertablemodfied : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ExternalOrderNumber", c => c.String());
            AddColumn("dbo.ResourceRequests", "HolidayEntitlement", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResourceRequests", "HolidayEntitlement");
            DropColumn("dbo.Orders", "ExternalOrderNumber");
        }
    }
}
