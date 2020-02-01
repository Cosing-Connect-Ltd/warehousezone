namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class marketexternalidadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Markets", "ExternalId", c => c.Int());
        }
            
        
        public override void Down()
        {
            DropColumn("dbo.Markets", "ExternalId");
        }
    }
}
