namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userverificationcodestableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthUserVerifyCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        VerifyCode = c.String(),
                        VerifyType = c.Int(nullable: false),
                        Expiry = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthUserVerifyCodes", "UserId", "dbo.AuthUsers");
            DropIndex("dbo.AuthUserVerifyCodes", new[] { "UserId" });
            DropTable("dbo.AuthUserVerifyCodes");
        }
    }
}
