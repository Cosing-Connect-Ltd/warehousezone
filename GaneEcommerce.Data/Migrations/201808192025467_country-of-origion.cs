namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countryoforigion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "CountryOfOrigion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "CountryOfOrigion");
        }
    }
}
