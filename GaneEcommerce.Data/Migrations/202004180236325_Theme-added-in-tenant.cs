namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Themeaddedintenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tenants", "Theme", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tenants", "Theme");
        }
    }
}
