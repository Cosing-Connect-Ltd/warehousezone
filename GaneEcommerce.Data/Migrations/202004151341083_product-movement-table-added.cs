namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productmovementtableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductMovements",
                c => new
                    {
                        ProductMovementId = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductMovementId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductMovements");
        }
    }
}
