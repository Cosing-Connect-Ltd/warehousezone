namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsCollectionFromCustomerSideinorders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsCollectionFromCustomerSide", c => c.Boolean(nullable: false));
            
        }
        
        public override void Down()
        {
            
            DropColumn("dbo.Orders", "IsCollectionFromCustomerSide");
        }
    }
}
