namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingshowingdecimalpointforordersintenantconfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "ShowDecimalPoint", c => c.Boolean(nullable: false));
           
        }
        
        public override void Down()
        {
            
            DropColumn("dbo.TenantConfigs", "ShowDecimalPoint");
        }
    }
}
