namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timberpropertiescorrection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProcesses", "FSC", c => c.String());
            AddColumn("dbo.OrderProcesses", "PEFC", c => c.String());
            AddColumn("dbo.OrderProcessDetails", "FscPercent", c => c.String());
            DropColumn("dbo.OrderProcessDetails", "FSC");
            DropColumn("dbo.OrderProcessDetails", "PEFC");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProcessDetails", "PEFC", c => c.String());
            AddColumn("dbo.OrderProcessDetails", "FSC", c => c.String());
            DropColumn("dbo.OrderProcessDetails", "FscPercent");
            DropColumn("dbo.OrderProcesses", "PEFC");
            DropColumn("dbo.OrderProcesses", "FSC");
        }
    }
}
