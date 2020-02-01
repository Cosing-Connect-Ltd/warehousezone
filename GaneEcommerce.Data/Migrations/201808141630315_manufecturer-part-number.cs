namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufecturerpartnumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "ManufacturerPartNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "ManufacturerPartNo");
        }
    }
}
