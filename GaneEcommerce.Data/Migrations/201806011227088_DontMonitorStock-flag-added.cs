namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DontMonitorStockflagadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "DontMonitorStock", c => c.Boolean());
            AddColumn("dbo.OrderDetails", "DontMonitorStock", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetails", "DontMonitorStock");
            DropColumn("dbo.ProductMaster", "DontMonitorStock");
        }
    }
}
