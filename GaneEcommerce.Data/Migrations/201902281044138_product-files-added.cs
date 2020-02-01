namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productfilesadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        FilePath = c.String(),
                        SortOrder = c.Short(nullable: false),
                        DefaultImage = c.Boolean(nullable: false),
                        HoverImage = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.ProductGroups", "IconPath", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductFiles", "ProductId", "dbo.ProductMaster");
            DropIndex("dbo.ProductFiles", new[] { "ProductId" });
            DropColumn("dbo.ProductGroups", "IconPath");
            DropTable("dbo.ProductFiles");
        }
    }
}
