namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAcceptedShelfLifeinaccounttable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "AcceptedShelfLife", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "AcceptedShelfLife");
        }
    }
}
