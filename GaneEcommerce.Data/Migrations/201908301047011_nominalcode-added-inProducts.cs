namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nominalcodeaddedinProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "NominalCode", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "NominalCode");
        }
    }
}
