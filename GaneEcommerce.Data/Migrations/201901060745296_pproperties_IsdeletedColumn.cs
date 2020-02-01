namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pproperties_IsdeletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PProperties", "IsDeleted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PProperties", "IsDeleted");
        }
    }
}
