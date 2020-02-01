namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableProductTaxWarranty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "EnableWarranty", c => c.Boolean());
            AddColumn("dbo.ProductMaster", "EnableTax", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "EnableTax");
            DropColumn("dbo.ProductMaster", "EnableWarranty");
        }
    }
}
