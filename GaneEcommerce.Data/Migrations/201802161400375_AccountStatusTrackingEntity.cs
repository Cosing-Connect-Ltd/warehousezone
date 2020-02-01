namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountStatusTrackingEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountStatusAudit",
                c => new
                    {
                        AccountStatusAuditId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Reason = c.String(),
                        LastStatusId = c.Int(nullable: false),
                        NewStatusId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AccountStatusAuditId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.GlobalAccountStatus", t => t.NewStatusId)
                .Index(t => t.AccountId)
                .Index(t => t.NewStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountStatusAudit", "NewStatusId", "dbo.GlobalAccountStatus");
            DropForeignKey("dbo.AccountStatusAudit", "AccountId", "dbo.Account");
            DropIndex("dbo.AccountStatusAudit", new[] { "NewStatusId" });
            DropIndex("dbo.AccountStatusAudit", new[] { "AccountId" });
            DropTable("dbo.AccountStatusAudit");
        }
    }
}
