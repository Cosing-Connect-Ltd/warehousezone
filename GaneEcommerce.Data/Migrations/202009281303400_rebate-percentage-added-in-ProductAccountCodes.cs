namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rebatepercentageaddedinProductAccountCodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAccountCodes", "RebatePercentage", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAccountCodes", "RebatePercentage");
        }
    }
}
