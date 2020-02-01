namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class batchexpiryaddedinOrderProcessDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcessDetails", "BatchNumber", c => c.String());
            AddColumn("dbo.OrderProcessDetails", "ExpiryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProcessDetails", "ExpiryDate");
            DropColumn("dbo.OrderProcessDetails", "BatchNumber");
        }
    }
}
