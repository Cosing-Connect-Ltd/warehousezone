namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedManufacturerCodeasMFGCodeinProductManufacturer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductManufacturers", "MFGCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductManufacturers", "MFGCode");
        }
    }
}
