namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrestaShopCountryIdadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GlobalCountry", "PrestaShopCountryId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GlobalCountry", "PrestaShopCountryId");
        }
    }
}
