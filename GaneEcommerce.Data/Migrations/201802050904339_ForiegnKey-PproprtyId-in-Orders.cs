namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForiegnKeyPproprtyIdinOrders : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "PPropertyId");
            RenameColumn(table: "dbo.Orders", name: "PProperties_PPropertyId", newName: "PPropertyId");
            RenameIndex(table: "dbo.Orders", name: "IX_PProperties_PPropertyId", newName: "IX_PPropertyId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Orders", name: "IX_PPropertyId", newName: "IX_PProperties_PPropertyId");
            RenameColumn(table: "dbo.Orders", name: "PPropertyId", newName: "PProperties_PPropertyId");
            AddColumn("dbo.Orders", "PPropertyId", c => c.Int());
        }
    }
}
