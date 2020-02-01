namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoOfCasesinOrderProofOfDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProofOfDeliveries", "NoOfCases", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProofOfDeliveries", "NoOfCases");
        }
    }
}
