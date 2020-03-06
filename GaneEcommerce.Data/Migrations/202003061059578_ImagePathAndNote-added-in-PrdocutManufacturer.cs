namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagePathAndNoteaddedinPrdocutManufacturer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductManufacturers", "ImagePath", c => c.String());
            AddColumn("dbo.ProductManufacturers", "Note", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductManufacturers", "Note");
            DropColumn("dbo.ProductManufacturers", "ImagePath");
        }
    }
}
