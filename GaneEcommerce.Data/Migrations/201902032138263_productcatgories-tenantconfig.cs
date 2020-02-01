namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productcatgoriestenantconfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "ProductCatagories", c => c.String());
          
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "ProductCatagories");
        }
    }
}
