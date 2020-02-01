namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedCashReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VanSalesDailyCashes", "SubmittedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSalesDailyCashes", "SubmittedDate");
        }
    }
}
