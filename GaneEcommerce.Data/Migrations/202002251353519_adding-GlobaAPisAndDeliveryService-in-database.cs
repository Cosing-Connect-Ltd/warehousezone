namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingGlobaAPisAndDeliveryServiceindatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GlobalApis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        ApiUrl = c.String(),
                        ApiKey = c.String(),
                        ApiTypes = c.Int(nullable: false),
                        ExpiryDate = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        TenantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TenantDeliveryServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NetworkCode = c.String(),
                        NetworkDescription = c.String(),
                        DeliveryMethod = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TenantDeliveryServices");
            DropTable("dbo.GlobalApis");
        }
    }
}
