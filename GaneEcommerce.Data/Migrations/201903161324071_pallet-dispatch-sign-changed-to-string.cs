namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class palletdispatchsignchangedtostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PalletsDispatches", "ReceiverSign", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PalletsDispatches", "ReceiverSign", c => c.Binary());
        }
    }
}
