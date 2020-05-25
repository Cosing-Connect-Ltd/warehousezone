namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class producttypeaddedinProductmaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "ProductType", c => c.Int(nullable: false));
            DropColumn("dbo.ProductMaster", "Kit");
            DropColumn("dbo.ProductMaster", "IsRawMaterial");
            DropColumn("dbo.ProductMaster", "GroupedProduct");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductMaster", "GroupedProduct", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "IsRawMaterial", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductMaster", "Kit", c => c.Boolean(nullable: false));
            DropColumn("dbo.ProductMaster", "ProductType");
        }
    }
}
