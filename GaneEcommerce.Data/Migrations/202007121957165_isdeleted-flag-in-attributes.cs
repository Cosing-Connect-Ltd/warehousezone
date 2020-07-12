namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isdeletedflaginattributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributes", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAttributes", "IsDeleted");
        }
    }
}
