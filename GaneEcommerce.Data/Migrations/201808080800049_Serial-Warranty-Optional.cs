namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialWarrantyOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSerialis", "WarrantyID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSerialis", "WarrantyID", c => c.Int(nullable: false));
        }
    }
}
