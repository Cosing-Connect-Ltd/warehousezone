namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtemplateschanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TenantEmailTemplates", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropIndex("dbo.TenantEmailTemplates", new[] { "InventoryTransactionTypeId" });
            AddColumn("dbo.TenantEmailTemplates", "NotificationType", c => c.Int(nullable: false));
            AddColumn("dbo.TenantEmailTemplates", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.TenantEmailTemplates", "CreatedBy", c => c.Int());
            AddColumn("dbo.TenantEmailTemplates", "IsDeleted", c => c.Boolean());
            DropColumn("dbo.TenantEmailTemplates", "TenantEmailConfigId");
            DropColumn("dbo.TenantEmailTemplates", "InventoryTransactionTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenantEmailTemplates", "InventoryTransactionTypeId", c => c.Int());
            AddColumn("dbo.TenantEmailTemplates", "TenantEmailConfigId", c => c.Int(nullable: false));
            DropColumn("dbo.TenantEmailTemplates", "IsDeleted");
            DropColumn("dbo.TenantEmailTemplates", "CreatedBy");
            DropColumn("dbo.TenantEmailTemplates", "DateCreated");
            DropColumn("dbo.TenantEmailTemplates", "NotificationType");
            CreateIndex("dbo.TenantEmailTemplates", "InventoryTransactionTypeId");
            AddForeignKey("dbo.TenantEmailTemplates", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes", "InventoryTransactionTypeId");
        }
    }
}
