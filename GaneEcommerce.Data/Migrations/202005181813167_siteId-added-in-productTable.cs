namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class siteIdaddedinproductTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "SiteId", c => c.Int());
           
        }
        
        public override void Down()
        {
            
            DropColumn("dbo.ProductMaster", "SiteId");
        }
    }
}
