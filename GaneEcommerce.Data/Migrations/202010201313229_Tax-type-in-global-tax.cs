namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Taxtypeinglobaltax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GlobalTax", "TaxType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GlobalTax", "TaxType");
        }
    }
}
