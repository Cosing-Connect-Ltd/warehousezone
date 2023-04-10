namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class palletTYpeIdaddedinpalletng : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pallets", "PalletTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pallets", "PalletTypeId");
        }
    }
}
