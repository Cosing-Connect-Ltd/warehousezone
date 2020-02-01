namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class plandlord_IsdeletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PLandlords", "IsDeleted", c => c.Boolean());
           
        }
        
        public override void Down()
        {
           
            DropColumn("dbo.PLandlords", "IsDeleted");
        }
    }
}
