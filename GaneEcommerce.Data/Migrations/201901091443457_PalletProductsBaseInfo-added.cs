namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PalletProductsBaseInfoadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PalletProducts", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.PalletProducts", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.PalletProducts", "DateUpdated", c => c.DateTime());
            AddColumn("dbo.PalletProducts", "UpdatedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PalletProducts", "UpdatedBy");
            DropColumn("dbo.PalletProducts", "DateUpdated");
            DropColumn("dbo.PalletProducts", "CreatedBy");
            DropColumn("dbo.PalletProducts", "DateCreated");
        }
    }
}
