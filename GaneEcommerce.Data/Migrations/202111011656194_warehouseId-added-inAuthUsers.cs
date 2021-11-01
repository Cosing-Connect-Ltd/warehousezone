namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class warehouseIdaddedinAuthUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthUsers", "WarehouseId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthUsers", "WarehouseId");
        }
    }
}
