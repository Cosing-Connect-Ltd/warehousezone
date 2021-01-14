namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDeliveryChargesForAudit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DeliveryCharges", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DeliveryCharges");
        }
    }
}
