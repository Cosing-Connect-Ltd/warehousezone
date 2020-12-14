namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDefaultImageAltTexttoProductMastertable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "DefaultImageAltText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "DefaultImageAltText");
        }
    }
}
