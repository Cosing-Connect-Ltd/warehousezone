namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsDeleted_columnfor_palletandpalletproduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletProducts", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Pallets", "IsDeleted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pallets", "IsDeleted");
            DropColumn("dbo.PalletProducts", "IsDeleted");
        }
    }
}
