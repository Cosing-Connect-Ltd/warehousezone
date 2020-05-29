namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalApichangedtoApiCredentials : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GlobalApis", newName: "ApiCredentials");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ApiCredentials", newName: "GlobalApis");
        }
    }
}
