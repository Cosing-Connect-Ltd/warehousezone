namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeRequiredRemoved : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductMaster", "BarCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductMaster", "BarCode", c => c.String(nullable: false));
        }
    }
}
