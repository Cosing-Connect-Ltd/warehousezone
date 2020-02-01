namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsStockItemflagadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "IsStockItem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "IsStockItem");
        }
    }
}
