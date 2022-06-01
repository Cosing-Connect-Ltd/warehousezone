namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class weightnullinproductmasters : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductMaster", "Weight", c => c.Double());
           
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductMaster", "Weight", c => c.Double(nullable: false));
        }
    }
}
