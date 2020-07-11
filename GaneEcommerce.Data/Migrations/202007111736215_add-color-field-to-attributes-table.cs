namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addcolorfieldtoattributestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributes", "IsColorTyped", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductAttributeValues", "Color", c => c.String());
            Sql("UPDATE dbo.ProductAttributes SET IsColorTyped = 1 WHERE AttributeName LIKE N'%Color%' OR AttributeName LIKE N'%Colour%'");
            Sql("UPDATE dbo.ProductAttributeValues SET Color = Value");
        }

        public override void Down()
        {
            DropColumn("dbo.ProductAttributeValues", "Color");
            DropColumn("dbo.ProductAttributes", "IsColorTyped");
        }
    }
}
