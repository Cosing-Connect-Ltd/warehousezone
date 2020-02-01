namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deliveryNoConsignmentNoSimplified : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderProcesses", "ConsignmentNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProcesses", "ConsignmentNumber", c => c.String());
        }
    }
}
