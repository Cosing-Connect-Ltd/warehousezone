namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAccountSectorstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountSectors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Account", "AccountSectorId", c => c.Int());
            CreateIndex("dbo.Account", "AccountSectorId");
            AddForeignKey("dbo.Account", "AccountSectorId", "dbo.AccountSectors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Account", "AccountSectorId", "dbo.AccountSectors");
            DropIndex("dbo.Account", new[] { "AccountSectorId" });
            DropColumn("dbo.Account", "AccountSectorId");
            DropTable("dbo.AccountSectors");
        }
    }
}
