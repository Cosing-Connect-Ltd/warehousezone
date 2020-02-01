namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowCasesPriceandAllowExportDatabaseflagsaddedinterminals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Terminals", "AllowExportDatabase", c => c.Boolean(nullable: false));
            AddColumn("dbo.Terminals", "ShowCasePrices", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Terminals", "ShowCasePrices");
            DropColumn("dbo.Terminals", "AllowExportDatabase");
        }
    }
}
