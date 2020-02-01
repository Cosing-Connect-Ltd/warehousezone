namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingpallettypeTableandpallettypeIdinproductmaser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PalletTypes",
                c => new
                    {
                        PalletTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PalletTypeId);
            
            AddColumn("dbo.ProductMaster", "PalletTypeId", c => c.Int());
            CreateIndex("dbo.ProductMaster", "PalletTypeId");
            AddForeignKey("dbo.ProductMaster", "PalletTypeId", "dbo.PalletTypes", "PalletTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductMaster", "PalletTypeId", "dbo.PalletTypes");
            DropIndex("dbo.ProductMaster", new[] { "PalletTypeId" });
            DropColumn("dbo.ProductMaster", "PalletTypeId");
            DropTable("dbo.PalletTypes");
        }
    }
}
