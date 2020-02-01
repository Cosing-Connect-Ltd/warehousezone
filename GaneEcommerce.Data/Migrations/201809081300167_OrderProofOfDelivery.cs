namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderProofOfDelivery : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderProofOfDeliveries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SignatoryName = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                        FileContent = c.Binary(),
                        OrderProcessID = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderProcesses", t => t.OrderProcessID)
                .ForeignKey("dbo.AuthUsers", t => t.CreatedBy)
                .Index(t => t.OrderProcessID)
                .Index(t => t.CreatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProofOfDeliveries", "CreatedBy", "dbo.AuthUsers");
            DropForeignKey("dbo.OrderProofOfDeliveries", "OrderProcessID", "dbo.OrderProcesses");
            DropIndex("dbo.OrderProofOfDeliveries", new[] { "CreatedBy" });
            DropIndex("dbo.OrderProofOfDeliveries", new[] { "OrderProcessID" });
            DropTable("dbo.OrderProofOfDeliveries");
        }
    }
}
