namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTooltiptable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tooltips",
                c => new
                    {
                        TooltipId = c.Int(nullable: false, identity: true),
                        Localization = c.String(),
                        Key = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        TenantId = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TooltipId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tooltips");
        }
    }
}
