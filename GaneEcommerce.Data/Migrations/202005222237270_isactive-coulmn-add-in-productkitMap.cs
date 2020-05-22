namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isactivecoulmnaddinproductkitMap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductKitMaps", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductKitMaps", "IsActive");
        }
    }
}
