namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class fixedApiCredentialsandOrdersrelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApiCredentials", "SiteTitle", c => c.String());
            AddColumn("dbo.Orders", "ApiCredentialId", c => c.Int());
            AlterColumn("dbo.SLAPriorits", "SortOrder", c => c.Int());
            CreateIndex("dbo.Orders", "SiteID");
            CreateIndex("dbo.Orders", "ApiCredentialId");
            AddForeignKey("dbo.Orders", "ApiCredentialId", "dbo.ApiCredentials", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ApiCredentialId", "dbo.ApiCredentials");
            DropIndex("dbo.Orders", new[] { "ApiCredentialId" });
            DropIndex("dbo.Orders", new[] { "SiteID" });
            AlterColumn("dbo.SLAPriorits", "SortOrder", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "ApiCredentialId");
            DropColumn("dbo.ApiCredentials", "SiteTitle");
        }
    }
}
